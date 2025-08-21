using UnityEngine;
using UnityEngine.UI;
using LLMUnity;
using TMPro;
using System;

public class Assignment1_Logic : MonoBehaviour
{
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
        Debug.Log(reply);
        textbox.text = reply;
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
