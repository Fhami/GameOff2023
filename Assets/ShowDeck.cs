using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class DeckView : MonoBehaviour
{

    public Canvas canvas;
    public bool showDeck = true;
    void Start()
    {
        
    }

    void Update()
    {
        
    }

    public void ShowDeck()
    {
        Debug.Log("Hellow from show deck");
        canvas = GetComponent<Canvas>();

        if (showDeck == true )
        {
            canvas.enabled = false;
            showDeck = false;
        }
        else
        {
            canvas.enabled = true;
            showDeck = true;
        }
    }
}
