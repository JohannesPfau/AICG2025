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
    [Tooltip("When to start cutting old characters from response.")]
    public int textThreshold = 512;

    [Header("Refs")]
    public LLMCharacter llmCharacter;
    public Text textbox;
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
        // string message = "Hello! Whats your name?";
        // _ = llmCharacter.Chat(message, HandleReply, ReplyCompleted);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void HandleReply(string reply){
        if (constrainOn)
            reply = ConstrainReply(reply);

        Debug.Log(reply);
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
