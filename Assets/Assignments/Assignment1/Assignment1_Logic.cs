//#define ConstrainDebug

using UnityEngine;
using UnityEngine.UI;
using LLMUnity;
using TMPro;
using System;
using NUnit.Framework;

public class Assignment1_Logic : MonoBehaviour
{
    [Header("Reply Constraining")]
    [Tooltip("You know what this does.")]
    public bool constrainOn = true;
    [Tooltip("Move text in <think> tags to thinkbox.")]
    public bool moveThinkingToThinkbox = true;
    [Tooltip("When to start cutting old characters from response.")]
    public int textThreshold = 512;

    [Header("Refs")]
    public LLMCharacter llmCharacter;
    public Text textbox;
    public Text thinkbox;
    public TMP_InputField globalChatInput;
    long timestampStart = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
    public GameObject loadingUI;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // globalChatInput.interactable = false;
        globalChatInput.onSubmit.AddListener(onInputFieldSubmit);
        Debug.Log(llmCharacter.prompt);
        textbox.text = "";
        thinkbox.text = "";
        // string message = "Hello! Whats your name?";
        // _ = llmCharacter.Chat(message, HandleReply, ReplyCompleted);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void HandleReply(string reply){
        Debug.Log(reply);
        if (moveThinkingToThinkbox)
        {
            string thinkingContent = "";
            
            while (reply.Contains("<think>"))
            {
                int startIndex = reply.IndexOf("<think>");
                int endIndex = reply.IndexOf("</think>", startIndex);
                
                if (endIndex != -1) {
                    string thinkText = reply.Substring(startIndex + 7, endIndex - startIndex - 7);
                    thinkingContent += thinkText + "\n";
                    reply = reply.Remove(startIndex, endIndex - startIndex + 8);
                }
                else {
                    // No closing tag found, extract everything from <think> to the end
                    string thinkText = reply.Substring(startIndex + 7);
                    thinkingContent += thinkText + "\n";
                    
                    // Remove everything from <think> to the end
                    reply = reply.Remove(startIndex);
                    break;
                }
            }
            
            // Handle case where there's only a closing </think> tag without opening <think>
            if (reply.Contains("</think>") && !reply.Contains("<think>")) {
                int endIndex = reply.IndexOf("</think>");
                string thinkText = reply.Substring(0, endIndex);
                thinkingContent += thinkText + "\n";
                
                // Remove everything before and including </think>
                reply = reply.Remove(0, endIndex + 8);
            }
            
            // Display the thinking content in thinkbox
            if (constrainOn)
                thinkingContent = ConstrainReply(thinkingContent);
            thinkbox.text = thinkingContent.TrimEnd('\n');
        }

        if (constrainOn)
            reply = ConstrainReply(reply);

        textbox.text = reply;
    }

    /// <summary>
    /// Constrains the reply so that the max_vertices threshold of the text renderer is not exceeded (hopefully)
    /// </summary>
    /// <param name="reply"></param>
    string ConstrainReply(string reply)
    {
#if ConstrainDebug
        Debug.Log("Characters: " + reply.Length.ToString());
        bool exceededFlag = reply.Length > textThreshold;
#endif
        string niceReply = (reply.Length > textThreshold) ? reply.Substring(reply.Length - textThreshold, textThreshold) : reply;

#if ConstrainDebug
        if (exceededFlag)
            Debug.Log("niceReply");
        if (niceReply.Length > textThreshold)
            Debug.Log("NOPE");
#endif

        Debug.Assert(niceReply.Length <= textThreshold);
        return niceReply;
    }

    void ReplyCompleted(){
        Debug.Log("The AI replied");
        
        llmCharacter.Save("Assignment1_chatlog" + timestampStart);
        globalChatInput.interactable = true;
        globalChatInput.Select();
        loadingUI.SetActive(false);
    }

    void onInputFieldSubmit(string message){
        Debug.Log(message);
        _ = llmCharacter.Chat(message, HandleReply, ReplyCompleted);
        globalChatInput.interactable = false;
        globalChatInput.text = "";
        loadingUI.SetActive(true);
    }
}
