using System;
using System.Collections;
using System.Collections.Generic;
using DefaultNamespace;
using UnityEngine;

/// <summary>
/// Use this to control Deck, Hand and Shrine
/// </summary>
public class CardController : MonoBehaviour
{
    public Deck Deck;
    public Hand Hand;
    public Shrine Shrine;

    [SerializeField] private float drawInterval = 0.1f;
    

    [SerializeField] private bool isDebug;

    private WaitForSeconds drawDelay;

    private void Awake()
    {
        drawDelay = new WaitForSeconds(drawInterval);
    }

    private void Start()
    {
        if (isDebug)
        {
            Database.Initialize();
            
            //Add cards to player deck
            foreach (var _cardData in Database.cardData)
            {
                GameManager.Instance.PlayerDeck.AddCard(CardFactory.Create(_cardData.Key));
            }

            foreach (var _card in GameManager.Instance.PlayerDeck.Cards)
            {
                //Create card object
                var _newCardObj = CardFactory.CreateCardObject(_card);
                
                Deck.AddCard(_newCardObj);
            }
            
            StartCoroutine(Draw(4));
        }
    }

    
    public IEnumerator Draw(int _number)
    {
        //Don't have enough card in deck, try reset shrine
        if (Deck.Cards.Count < _number)
        {
            foreach (var _card in Shrine.GetCards(-1))
            {
                Deck.AddCard(_card);
                
                yield return drawDelay;
            }
            Deck.Shuffle();
        }

        foreach (var _card in Deck.GetCards(_number))
        {
            Hand.AddCard(_card);
            
            _card.transform.position = Deck.transform.position;
            
            Hand.Cards.Add(_card);
            
            yield return drawDelay;
            
            //Set parent to hand transform
            //Draw animation will auto play by Layout group when SetParent
            _card.transform.SetParent(Hand.Container);
            _card.transform.SetAsFirstSibling();
        }
        
    }
}
