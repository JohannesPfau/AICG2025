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
    public Text namebox;
    public TMP_InputField globalChatInput;
    long timestampStart = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
    public GameObject loadingUI;
    public GameObject conversationPartner;
    public bool isConversationInitiator = false;
    string currentText = "";

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // globalChatInput.interactable = false;
        if (!globalChatInput)
            globalChatInput = GameObject.FindGameObjectWithTag("GlobalChatbox").GetComponentInChildren<TMP_InputField>();
        globalChatInput.onSubmit.AddListener(onInputFieldSubmit);

        // llmCharacter.prompt += "\nIMPORTANT: After every message, add a <expression> tag with the expression of the character, based on the content of the message. The expression can be Joy, Sad, Angry, Surprised, or Neutral.";
        Debug.Log(llmCharacter.prompt);
        if(!textbox)
            textbox = GetComponentInChildren<Text>();
        textbox.text = "";
        if(!namebox)
            namebox = GetComponentsInChildren<Text>()[1];
        namebox.text = GetComponent<LLMCharacter>().AIName;
        // string message = "Hello! Whats your name?";
        // _ = llmCharacter.Chat(message, HandleReply, ReplyCompleted);
        if(!conversationPartner)
            // Find the closest GameObject tagged "NPC" to this one, skipping those with a conversation partner already set
            {
                GameObject[] npcs = GameObject.FindGameObjectsWithTag("NPC");
                GameObject closest = null;
                float minDist = float.MaxValue;
                Vector3 myPos = transform.position;
                foreach (GameObject npc in npcs)
                {
                    if (npc == this.gameObject) continue;
                    Assignment1_Logic npcLogic = npc.GetComponent<Assignment1_Logic>();
                    if (npcLogic != null && npcLogic.conversationPartner != null) continue;
                    float dist = Vector3.Distance(myPos, npc.transform.position);
                    if (dist < minDist)
                    {
                        minDist = dist;
                        closest = npc;
                    }
                }
                conversationPartner = closest;
                closest.GetComponent<Assignment1_Logic>().conversationPartner = this.gameObject;
                transform.LookAt(conversationPartner.transform);
                conversationPartner.transform.LookAt(this.transform);
                isConversationInitiator = true;

                // Find which number this NPC is of all NPC GameObjects, then delay conversation start
                int myIndex = -1;
                for (int i = 0; i < npcs.Length; i++)
                {
                    if (npcs[i] == this.gameObject)
                    {
                        myIndex = i;
                        break;
                    }
                }
                Debug.Log("This NPC's index: " + myIndex + ", starting conversation after: " + myIndex * 1);
                Invoke(nameof(startConversation), myIndex * 1);
            }
    }

    // Update is called once per frame
    void Update()
    {
        // Find all Canvas components in children and set their global Y rotation to face the camera
        Canvas[] canvases = GetComponentsInChildren<Canvas>(true);
        foreach (Canvas canvas in canvases)
        {
            Vector3 euler = canvas.transform.parent.rotation.eulerAngles;
            canvas.transform.parent.rotation = Quaternion.Euler(euler.x, 0f, euler.z);
        }
    }

    void startConversation()
    {
        if(isConversationInitiator) {
            if(GameObject.FindGameObjectWithTag("LLM").GetComponent<LLMLogic>().isBusy) {
                Invoke(nameof(startConversation), 5);
                return;
            }

            GameObject.FindGameObjectWithTag("LLM").GetComponent<LLMLogic>().isBusy = true;
            Debug.Log("Starting conversation");
            string message = "Hello! Who are you? Tell me something about yourself!";
            currentText = message;
            textbox.text = message;
            Debug.Log(message);
            conversationPartner.GetComponent<Assignment1_Logic>().onInputFieldSubmit(message);
            // _ = llmCharacter.Chat(message, HandleReply, ReplyCompleted);
            // loadingUI.SetActive(true);
        }
    }

    public void HandleReply(string reply){
        //Debug.Log(reply);
        currentText = reply;
        string textboxContent = "";
        
        while (reply.Contains("<think>"))
        {
            int startIndex = reply.IndexOf("<think>");
            int endIndex = reply.IndexOf("</think>", startIndex);
            
            if (endIndex != -1) {
                string thinkText = reply.Substring(startIndex + 7, endIndex - startIndex - 7);
                textboxContent += "<color=#36FFAD>" + thinkText + "</color>\n";
                reply = reply.Remove(startIndex, endIndex - startIndex + 8);
            }
            else {
                // No closing tag found, extract everything from <think> to the end
                string thinkText = reply.Substring(startIndex + 7);
                textboxContent += "<color=#36FFAD>" + thinkText + "</color>\n";
                
                // Remove everything from <think> to the end
                reply = reply.Remove(startIndex);
                break;
            }
        }
        
        // Handle case where there's only a closing </think> tag without opening <think>
        if (reply.Contains("</think>") && !reply.Contains("<think>")) {
            int endIndex = reply.IndexOf("</think>");
            string thinkText = reply.Substring(0, endIndex);
            textboxContent += "<color=#36FFAD>" + thinkText + "</color>\n";
            
            // Remove everything before and including </think>
            reply = reply.Remove(0, endIndex + 8);
        }
        
        // Add the thinking content to textbox with color formatting
        textboxContent += reply;
        if (constrainOn)
            textboxContent = ConstrainReply(textboxContent);
        textbox.text = textboxContent;
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

    void ReplyCompleted()
    {
        Debug.Log(GetComponent<LLMCharacter>().AIName + " : " + currentText);
        GameObject.FindGameObjectWithTag("LLM").GetComponent<LLMLogic>().isBusy = false;

        llmCharacter.Save("Assignment1_chatlog" + timestampStart);
        globalChatInput.interactable = true;
        globalChatInput.Select();
        loadingUI.SetActive(false);

        // TODO: Get proper expression from the LLM#
        //GetComponent<Assignment2_Logic>().SetRandomExpression();
        GetComponent<Assignment2_Logic>().ClassifyEmotion(currentText);
        
        if(conversationPartner)
            conversationPartner.GetComponent<Assignment1_Logic>().onInputFieldSubmit(currentText);
    }

    void onInputFieldSubmit(string message){
        //Debug.Log(message);
        _ = llmCharacter.Chat(message, HandleReply, ReplyCompleted);
        globalChatInput.interactable = false;
        globalChatInput.text = "";
        loadingUI.SetActive(true);
    }
}
