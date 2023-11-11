using System;
using System.Collections;
using System.Collections.Generic;
using DefaultNamespace;
using TMPro;
using UnityEngine;

public abstract class CardList : MonoBehaviour
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
    public virtual IEnumerable<Card> GetCards(int _number = -1)
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

    public virtual void Shuffle()
    {
        Cards.Shuffle();
    }
    
    public virtual void AddCards(IEnumerable<Card> _cards)
    {
        foreach (var _card in _cards)
        {
            Cards.Add(_card);
        }
    }
    
    public virtual void AddCard(Card _card)
    {
        Cards.Add(_card);
        
        UpdateCount(Cards.Count);
        OnValueChanged?.Invoke();
    }

    public virtual void RemoveCards(IEnumerable<Card> _cards)
    {
        foreach (var _card in _cards)
        {
            RemoveCard(_card);
        }
    }
    
    public virtual void RemoveCard(Card _card)
    {
        Cards.Remove(_card);
        
        UpdateCount(Cards.Count);
        OnValueChanged?.Invoke();
    }

    public virtual void Clear()
    {
        Cards.Clear();
        
        UpdateCount(Cards.Count);
        OnValueChanged?.Invoke();
    }
}
