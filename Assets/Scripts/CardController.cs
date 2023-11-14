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
    public CardPile Deck;
    public CardPile Hand;
    public CardPile Shrine;

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
        if (Deck.Cards.Count < _number)
        {
            foreach (var _recycledCard in Shrine.GetCards(-1))
            {
                Deck.AddCard(_recycledCard);
                
                yield return drawDelay;
            }
            Deck.Shuffle();
        }
        
        foreach (var _card in Deck.GetCards(_number))
        {
            Hand.AddCard(_card);
            _card.UpdateCard(Character.runtimeCharacter);
            
            _card.transform.position = Deck.transform.position;
            
            yield return drawDelay;
            
            //Set parent to hand transform
            //Draw animation will auto play by Layout group when SetParent
            
            _card.SetValidTarget(GetValidTargets(_card.runtimeCard));
        }
        
        foreach (var _card in Hand.Cards)
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
                        _target.runtimeCharacter, BattleManager.current.enemies.Select(_e => _e.runtimeCharacter).ToList()));
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
        Hand.RemoveCard(_card);
        Shrine.AddCard(_card);
        _card.ClearCallBack();

        yield return drawDelay;
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
    
    public IEnumerator ClearHand()
    {
        foreach (var _card in Hand.GetCards(-1))
        {
            if (_card)
            {
                Shrine.AddCard(_card);
                _card.ClearCallBack();

                yield return drawDelay;
            }
        }
    }
}
