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
        public int MaxSize = 10;
        public List<RuntimeCard> Cards = new List<RuntimeCard>();
        public List<RuntimeCard> SpecialCards = new List<RuntimeCard>();

        public void AddCard(RuntimeCard _card)
        {
            //Deck is full
            if (Cards.Count > MaxSize)
            {
                Debug.LogError("Deck is full!");
                return;
            }
            
            if (!Cards.Contains(_card))
            {
                Cards.Add(_card);
            }
        }

        /// <summary>
        /// Call when card got destroyed after used
        /// </summary>
        /// <param name="_card"></param>
        public void RemoveCard(RuntimeCard _card)
        {
            Cards.Remove(_card);
        }
    }
}
