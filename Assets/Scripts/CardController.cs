using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DefaultNamespace;
using DG.Tweening;
using UnityEngine;

/// <summary>
/// Use this to control Deck, Hand and Shrine
/// </summary>
public class CardController : MonoBehaviour
{
    public static bool ShowLog = false;
    
    public CardPile DeckPile;
    public CardPile HandPile;
    public CardPile DiscardPile;
    public CardPile ExhaustPile;

    public Character Character;
    
    [SerializeField] private float drawInterval = 0.1f;

    private WaitForSeconds drawDelay;

    private IEnumerable<CardPile> AllPiles
    {
        get
        {
            yield return DeckPile;
            yield return HandPile;
            yield return DiscardPile;
            yield return ExhaustPile;
        }
    }

    private void Awake()
    {
        drawDelay = new WaitForSeconds(drawInterval);
    }

    private void Update()
    {
        UpdateCardRotations();
    }

    public IEnumerator InitializeDeck(RuntimeDeckData _deck)
    {
        DeckPile.Clear();
        
        foreach (var _card in _deck.Cards)
        {
            yield return BattleManager.current.CreateCardAndAddItToDrawPile(CardFactory.Create(_card.name));
        }

        DeckPile.Shuffle();
    }
    
    public IEnumerator Draw(int _number)
    {
        foreach (var _card in DeckPile.GetCards(_number))
        {
            HandPile.AddCard(_card, false, Vector3.one, true);
            
            _card.UpdateCard(Character.runtimeCharacter);
            _card.transform.position = DeckPile.transform.position;

            yield return drawDelay;
            _card.transform.localScale = Vector3.one;
        }
    }

    /// <summary>
    /// Update cards description and values
    /// </summary>
    public void UpdateCards()
    {
        foreach (var _card in HandPile.Cards)
        {
            _card.UpdateCard(Character.runtimeCharacter);
        }
    }

    public IEnumerator ShuffleDiscardPileIntoDeck()
    {
        foreach (var _recycledCard in DiscardPile.GetCards(-1))
        {
            DeckPile.AddCard(_recycledCard, true, Vector3.zero);
                
            yield return null;
        }
        DeckPile.Shuffle();
    }

    public IEnumerator Discard(Card _card)
    {
        if (!_card) yield break;
        
        DiscardPile.AddCard(HandPile.PickCard(_card), true, Vector3.zero);

        yield return drawDelay;
    }

    public IEnumerator ExhaustCard(Card _card)
    {
        if (!_card) yield break;
        
        yield return _card.ExhaustCard();
        
        ExhaustPile.AddCard(HandPile.PickCard(_card), false, Vector3.zero);
    }
    
    public IEnumerator DestroyCard(Card _card)
    {
        if (!_card) yield break;
        
        foreach (var _pile in AllPiles)
        {
            _pile.RemoveCard(_card);
        }
        
        GameManager.Instance.PlayerRuntimeDeck.RemoveCard(_card.runtimeCard.cardData);
        
        yield return _card.DestroyCard();
        // E.g. SLIME card destroys itself when player changes size, you can debug using that.
    }

    public void ClearAllCards()
    {
        foreach (var _pile in AllPiles)
        {
            foreach (var _card in _pile.GetCards())
            {
                Destroy(_card.gameObject);
            }
        }
    }
    
    public IEnumerator DiscardRemainingCards()
    {
        var cardToDiscard = 0;
        var handCount = HandPile.Cards.Count;
        HashSet<Card> persistedCard = new HashSet<Card>();
        
        foreach (var card in HandPile.Cards)
        {
            if (card.runtimeCard.IsPersist())
            {
                persistedCard.Add(card);
            }
            else
            {
                cardToDiscard++;
            }
        }

        while (HandPile.Cards.Count > handCount - cardToDiscard)
        {
            var index = 0;
            Card card = HandPile.Cards[index];

            while (persistedCard.Contains(card))
            {
                index++;
                card = HandPile.Cards[index];
            }

            yield return BattleManager.current.DiscardCard(
                card.runtimeCard, 
                BattleManager.current.runtimePlayer,
                BattleManager.current.runtimePlayer, 
                null,
                BattleManager.current.runtimeEnemies);
        }
    }

    public IEnumerator CreateCardAndAddItToDrawPile(RuntimeCard runtimeCard)
    {
        // Create card object
        var _newCardObj = CardFactory.CreateCardObject(runtimeCard);
                
        AddCardListeners(_newCardObj);
            
        DeckPile.AddCard(_newCardObj, false, Vector3.zero);
        
        yield break;
    }
    
    public IEnumerator CreateCardAndAddItToDiscardPile(RuntimeCard runtimeCard)
    {
        // Create card object
        var _newCardObj = CardFactory.CreateCardObject(runtimeCard);
           
        AddCardListeners(_newCardObj);
        
        DiscardPile.AddCard(_newCardObj, false, Vector3.zero);
        
        yield break;
    }

    public IEnumerator CreateCardAndAddItToHand(RuntimeCard runtimeCard)
    {
        var _newCardObj = CardFactory.CreateCardObject(runtimeCard);
        
        AddCardListeners(_newCardObj);
        
        HandPile.AddCard(_newCardObj, false, Vector3.one, true);
        
        yield return drawDelay;
    }

    public IEnumerator ShuffleHandToDeck(RuntimeCard card)
    {
        DeckPile.AddCard(HandPile.PickCard(card.Card), true, Vector3.zero);

        yield return drawDelay;
    }
    
    private void AddCardListeners(Card card)
    {
        card.OnDropped.AddListener(_target =>
        {
            if (!BattleManager.current.canPlayCard) return;
            
            if (card.ValidateTarget(_target))
            {
                if (ShowLog)
                {
                    Debug.Log($"target {_target.GameObject.name} = true");
                }
                
                card.transform.SetParent(null);
                card.transform.DOMove(new Vector3(0, 1, 0), 0.2f);
                card.transform.DOScale(new Vector3(1.03f, 1.03f, 1f), 0.1f);
                card.Collider.enabled = false;

                var _targetChar = _target.GameObject.GetComponent<Character>();

                var _runtimeCharacter = _targetChar ? _targetChar.runtimeCharacter : null;

                StartCoroutine(BattleManager.current.PlayCard(card.runtimeCard, Character.runtimeCharacter, _runtimeCharacter, BattleManager.current.runtimeEnemies));
            }
            else
            {
                if (ShowLog)
                {
                    Debug.Log($"target {_target.GameObject.name} = false");
                }
            }
        });
    }
    
    private void UpdateCardRotations() 
    {
        int numCards = HandPile.Cards.Count;
        float totalAngleSpread = HandPile.Cards.Count * 2.5f;
        float rotateSpeed = 2.5f;
        
        for (int i = 0; i < numCards; i++) 
        {
            Quaternion cardRotation = CalculateRotationForCard(i, numCards, totalAngleSpread);
            HandPile.Cards[i].transform.rotation = Quaternion.Slerp(HandPile.Cards[i].transform.rotation, cardRotation, Time.deltaTime * rotateSpeed);
        }
    }

    private Quaternion CalculateRotationForCard(int cardIndex, int totalCards, float angleSpread) 
    {
        float anglePerCard = angleSpread / Mathf.Max(1, totalCards - 1);
        float angleForThisCard = -angleSpread / 2 + anglePerCard * cardIndex;
        Quaternion rotation = Quaternion.Euler(0, 0, angleForThisCard);
        return Quaternion.Normalize(rotation); // Normalize the quaternion
    }
}
