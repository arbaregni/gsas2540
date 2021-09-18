using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class TextCounter : MonoBehaviour
{
    [SerializeField]
    string pluralTemplate = "Your score: {}";
    [SerializeField]
    string singularTemplate = "Your score: {}";

    private Text text;
    private int count;

    public int Count
    {
        get => count;
   
        set
        {
            count = value;
            string template = count == 1 ? singularTemplate : pluralTemplate;
            text.text = template.Replace("{}", count.ToString());
        }
    
    }

    private void Awake()
    {
        text = GetComponent<Text>();
        Count = 0;
    }
}
