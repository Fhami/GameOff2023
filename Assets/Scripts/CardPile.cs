using System;
using System.Collections;
using System.Collections.Generic;
using DefaultNamespace;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class CardPile : MonoBehaviour
{
    public List<Card> Cards = new List<Card>();

    public Transform Container;

    public Action OnValueChanged;
    
    [SerializeField] private TextMeshPro count;

    public void UpdateCount(int _value)
    {
        if (count)
        {
            count.SetText(_value.ToString());
        }
    }
    
    /// <summary>
    /// Get cards from list
    /// </summary>
    /// <param name="_number">Number of card, -1 = all</param>
    /// <returns></returns>
    public IEnumerable<Card> GetCards(int _number = -1)
    {
        //number is -1, get all cards
        if (_number == -1)
        {
            while (Cards.Count > 0)
            {
                yield return Cards[0];

                RemoveCard(Cards[0]);
            }
            
            yield break;
        }
        
        while (_number > 0)
        {
            if (Cards.Count > 0)
            {
                yield return Cards[0];

                RemoveCard(Cards[0]);
                _number--;
            }
            else
            {
                //Empty
                break;
            }
        }
    }

    /// <summary>
    /// This may seems a bit weired, but thinking of picking up card from pile,
    /// we get card and card was removed from pile
    /// </summary>
    /// <param name="_card"></param>
    /// <returns></returns>
    public Card PickCard(Card _card)
    {
        if (Cards.Contains(_card))
        {
            RemoveCard(_card);
            return _card;
        }

        if (_card)
        {
            Debug.LogError($"Card {_card.name} doesn't exist in {name} pile!");
        }
        
        return null;
    } 

    public void Shuffle()
    {
        Cards.Shuffle();
        if (CardController.ShowLog)
            Debug.Log($"Shuffled {name}");
    }
    
    public void AddCards(IEnumerable<Card> _cards)
    {
        foreach (var _card in _cards)
        {
            Cards.Add(_card);
        }
    }
    
    public void AddCard(Card _card, bool _tween, Vector3 _scaleTween, bool _canInteract = false)
    {
        Cards.Add(_card);
        _card.Collider.enabled = _canInteract;

        if (Container)
        {
            //Set position to pile
            _card.transform.SetParent(Container);
            if (_tween)
            {
                _card.transform.DOLocalMove(Vector3.zero, 0.2f);
                _card.transform.DOScale(_scaleTween, 0.1f);
            }
            else
            {
                _card.transform.localPosition = Vector3.zero;
                _card.transform.localScale = _scaleTween;
            }
            _card.transform.SetAsFirstSibling(); //Put newly draw card to left side
        }
        
        UpdateCount(Cards.Count);
        OnValueChanged?.Invoke();
        
        if (CardController.ShowLog)
            Debug.Log($"Added {_card.name} to {name}");
    }

    public void RemoveCards(IEnumerable<Card> _cards)
    {
        foreach (var _card in _cards)
        {
            RemoveCard(_card);
        }
    }
    
    public void RemoveCard(Card _card)
    {
        Cards.Remove(_card);
        
        UpdateCount(Cards.Count);
        OnValueChanged?.Invoke();
        
        if (CardController.ShowLog)
            Debug.Log($"Removed {_card.name} from {name}");
    }

    public void Clear()
    {
        Cards.Clear();
        
        UpdateCount(Cards.Count);
        OnValueChanged?.Invoke();
    }
}
