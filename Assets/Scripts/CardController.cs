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
    public Deck Deck;
    public Hand Hand;
    public Shrine Shrine;

    public Character Character;
    
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
            
            yield return drawDelay;
            
            //Set parent to hand transform
            //Draw animation will auto play by Layout group when SetParent
            _card.transform.SetParent(Hand.Container);
            _card.transform.SetAsFirstSibling(); //Put newly draw card to left side
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
                    // StartCoroutine(BattleManager.current.PlayCard(_card.runtimeCard, Character.runtimeCharacter,
                    //     _target.runtimeCharacter, BattleManager.current.enemies.Select(_e => _e.runtimeCharacter).ToList()));
                }
                else
                {
                    Debug.Log($"target = false");
                }
            });
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
