using DefaultNamespace;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.Rendering;

public class DeckView : MonoBehaviour
{

    public Canvas canvas;
    public GameObject cardContainer;
    public bool showDeck = false;

    public List<CardUI> CurrentCard;
    public List <CardData> Card;

    public void ShowDeck()
    {
        canvas = GetComponent<Canvas>();

        if (showDeck == true )
        {
            canvas.enabled = false;
            showDeck = false;
            foreach (var card in CurrentCard)
            {
                Destroy(card.gameObject);
            }
            CurrentCard.Clear();
            
        }
        else
        {
            canvas.enabled = true;
            showDeck = true;

            foreach (var card in GameManager.Instance.PlayerRuntimeDeck.Cards)
            {
                CardUI cardUI = CardFactory.CreateCardUI(card.name);
                cardUI.transform.SetParent(cardContainer.transform, false);
                CurrentCard.Add(cardUI);
            }
        }
    }
}
