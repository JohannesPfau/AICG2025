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

    public void SetExpression(string expression)
    {
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
