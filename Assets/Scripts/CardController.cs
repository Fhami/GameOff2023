using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DefaultNamespace;
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

    public IEnumerator InitializeDeck(RuntimeDeckData _deck)
    {
        DeckPile.Clear();
        
        foreach (var _card in _deck.Cards)
        {
            yield return BattleManager.current.CreateCardAndAddItToDrawPile(_card);
        }

        DeckPile.Shuffle();
    }
    
    public IEnumerator Draw(int _number)
    {
        foreach (var _card in DeckPile.GetCards(_number))
        {
            HandPile.AddCard(_card);
            
            _card.UpdateCard(Character.runtimeCharacter);
            _card.transform.position = DeckPile.transform.position;

            yield return drawDelay;
        }
    }

    public IEnumerator ShuffleDiscardPileIntoDeck()
    {
        foreach (var _recycledCard in DiscardPile.GetCards(-1))
        {
            DeckPile.AddCard(_recycledCard);
                
            yield return drawDelay;
        }
        DeckPile.Shuffle();
    }

    public IEnumerator Discard(Card _card)
    {
        DiscardPile.AddCard(HandPile.PickCard(_card));

        yield return drawDelay;
    }

    public IEnumerator ExhaustCard(Card _card)
    {
        yield return _card.ExhaustCard();
        
        ExhaustPile.AddCard(HandPile.PickCard(_card));
    }
    
    public IEnumerator DestroyCard(Card _card)
    {
        // throw new NotImplementedException(
        //     " // TODO: Can we destroy cards from any card pile? Can we handle that?\n" + 
        //     "// TODO: Add card destroy VFX\n" + 
        //     "// TODO: Also remove runtime card data from player, and destroy the card gameobject\n" +
        //     "// TODO: IDK what to do here haha. Kamee halp?");

        foreach (var _pile in AllPiles)
        {
            _pile.RemoveCard(_card);
        }
        
        GameManager.Instance.PlayerRuntimeDeck.RemoveCard(_card.runtimeCard);
        
        yield return _card.DestroyCard();
        // E.g. SLIME card destroys itself when player changes size, you can debug using that.
    }
    
    public IEnumerator DiscardRemainingCards()
    {
        while (HandPile.Cards.Count > 0)
        {
            Card card = HandPile.Cards[0];
            
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
            
        DeckPile.AddCard(_newCardObj);
        
        yield break;
    }
    
    public IEnumerator CreateCardAndAddItToDiscardPile(RuntimeCard runtimeCard)
    {
        // Create card object
        var _newCardObj = CardFactory.CreateCardObject(runtimeCard);
           
        AddCardListeners(_newCardObj);
        
        DiscardPile.AddCard(_newCardObj);
        
        yield break;
    }
    
    private void AddCardListeners(Card card)
    {
        card.OnDropped.AddListener(_target =>
        {
            if (card.ValidateTarget(_target))
            {
                if (ShowLog)
                {
                    Debug.Log($"target {_target.GameObject.name} = true");
                }
                    
                card.transform.SetParent(null);

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
}
