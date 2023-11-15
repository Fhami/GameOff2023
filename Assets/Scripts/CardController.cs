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
    public CardPile DeckPile;
    public CardPile HandPile;
    public CardPile DiscardPile;
    public CardPile ExhaustPile;

    public Character Character;
    
    [SerializeField] private float drawInterval = 0.1f;
    

    [SerializeField] private bool isDebug;

    private WaitForSeconds drawDelay;

    private void Awake()
    {
        drawDelay = new WaitForSeconds(drawInterval);
    }
    
    public IEnumerator Draw(int _number)
    {
        //Don't have enough card in deck, try reset shrine
        if (DeckPile.Cards.Count < _number)
        {
            foreach (var _recycledCard in DiscardPile.GetCards(-1))
            {
                DeckPile.AddCard(_recycledCard);
                
                yield return drawDelay;
            }
            DeckPile.Shuffle();
        }
        
        foreach (var _card in DeckPile.GetCards(_number))
        {
            HandPile.AddCard(_card);
            _card.UpdateCard(Character.runtimeCharacter);
            
            _card.transform.position = DeckPile.transform.position;
            
            yield return drawDelay;
            
            //Set parent to hand transform
            //Draw animation will auto play by Layout group when SetParent
            
            _card.SetValidTarget(GetValidTargets(_card.runtimeCard));
        }
        
        foreach (var _card in HandPile.Cards)
        {
            _card.OnDrag.AddListener(_arg0 =>
            {
                //TODO: Highlight valid target
                
            });
            
            _card.OnDropped.AddListener(_target =>
            {
                if (_card.ValidateTarget(_target))
                {
                    Debug.Log($"target = true");
                    
                    _card.transform.SetParent(null);
                    
                    StartCoroutine(BattleManager.current.PlayCard(_card.runtimeCard, Character.runtimeCharacter,
                        _target.runtimeCharacter, BattleManager.current.runtimeEnemies));
                }
                else
                {
                    Debug.Log($"target = false");
                }
            });
        }
        
    }

    public IEnumerator Discard(Card _card)
    {
        DiscardPile.AddCard(HandPile.GetCard(_card));
        _card.ClearCallBack();

        yield return drawDelay;
    }

    public IEnumerator ExhaustCard(Card _card)
    {
        ExhaustPile.AddCard(HandPile.GetCard(_card));
        _card.ClearCallBack();

        yield return drawDelay;
    }
    
    public IEnumerator ClearHand()
    {
        foreach (var _card in HandPile.GetCards(-1))
        {
            if (_card)
            {
                DiscardPile.AddCard(_card);
                _card.ClearCallBack();

                yield return drawDelay;
            }
        }
    }
    
    public List<Character> GetValidTargets(RuntimeCard _runtimeCard)
    {
        var _results = new List<Character>();
        var _targetTags = _runtimeCard.cardData.cardDragTarget;
        foreach (var _tag in Enum.GetValues(_targetTags.GetType()))
        {
            if (_tag.ToString() == "NONE") continue;
                
            var _targets = GameObject.FindGameObjectsWithTag(_tag.ToString());
                
            foreach (var _target in _targets)
            {
                if (_target.TryGetComponent<Character>(out var _character))
                {
                    _results.Add(_character);
                }
            }
        }

        return _results;
    }
}
