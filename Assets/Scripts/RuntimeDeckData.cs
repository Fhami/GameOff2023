using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DefaultNamespace
{
    /// <summary>
    /// Contain list of cards in deck
    /// </summary>
    public class RuntimeDeckData
    {
        public int MaxSize = 9999;
        public List<CardData> Cards = new List<CardData>();
        public List<CardData> SpecialCards = new List<CardData>();

        public void AddCard(CardData _card)
        {
            //Deck is full
            if (Cards.Count > MaxSize)
            {
                Debug.LogError("Deck is full!");
                return;
            }

            
                Cards.Add(_card);
        }

        /// <summary>
        /// Call when card got destroyed after used
        /// </summary>
        /// <param name="_card"></param>
        public void RemoveCard(CardData _card)
        {
            Cards.Remove(_card);
        }
    }
}
