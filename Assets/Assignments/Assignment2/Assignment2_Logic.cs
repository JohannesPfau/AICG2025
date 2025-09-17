using LLMUnity;
using UnityEngine;
using UnityEngine.UI;

public class Assignment2_Logic : MonoBehaviour
{
    public Sprite sprite_Neutral;
    public Sprite sprite_Joy;
    public Sprite sprite_Sad;
    public Sprite sprite_Angry;
    public Sprite sprite_Surprised;
    public Transform expressionTransform;
    public string currentExpression = "Neutral";
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if(!expressionTransform)
            expressionTransform = transform.Find("Expression");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ClassifyEmotion(string message)
    {
        GameObject.FindGameObjectWithTag("EmotionClassifier").GetComponent<LLMCharacter>().Chat(message, EstimateExpression, HandleExpression);
        //GameObject.FindGameObjectWithTag("EmotionClassifier").GetComponent<LLMCharacter>().Chat("WOW What was that??", EstimateExpression, HandleExpression); // correctly resolves to "Surprised"
        //GameObject.FindGameObjectWithTag("EmotionClassifier").GetComponent<LLMCharacter>().Chat("I hate my life, everything is so depressing.", EstimateExpression, HandleExpression); // correctly resolves to "Sad"
        //GameObject.FindGameObjectWithTag("EmotionClassifier").GetComponent<LLMCharacter>().Chat("I WANT TO DESTROY EVERYTHING!", EstimateExpression, HandleExpression); // correctly resolves to "Angry"
        //GameObject.FindGameObjectWithTag("EmotionClassifier").GetComponent<LLMCharacter>().Chat("I'm having so much fun in this lecture!!!", EstimateExpression, HandleExpression); // correctly resolves to "Joy"
        //GameObject.FindGameObjectWithTag("EmotionClassifier").GetComponent<LLMCharacter>().Chat("I'm whatever.", EstimateExpression, HandleExpression); // correctly resolves to "Neutral"
    }

    public void EstimateExpression(string message)
    {
        currentExpression = message;
    }

    public void HandleExpression()
    {
        SetExpression(currentExpression);
    }

    public void SetExpression(string expression)
    {
        Debug.Log("Trying to set expression to: " + expression);
        if (expressionTransform)
        {
            Sprite selectedSprite;
            switch (expression)
            {
                case "Joy":
                    selectedSprite = sprite_Joy;
                    break;
                case "Sad":
                    selectedSprite = sprite_Sad;
                    break;
                case "Angry":
                    selectedSprite = sprite_Angry;
                    break;
                case "Surprised":
                    selectedSprite = sprite_Surprised;
                    break;
                default:
                    selectedSprite = sprite_Neutral;
                    break;
            }
            expressionTransform.GetComponent<Image>().sprite = selectedSprite;
        }
    }

    public void SetRandomExpression()
    {
        string[] expressions = { "Neutral", "Joy", "Sad", "Angry", "Surprised" };
        int randomIndex = UnityEngine.Random.Range(0, expressions.Length);
        SetExpression(expressions[randomIndex]);
        Debug.Log("Set random expression: " + expressions[randomIndex]);
    }
}
