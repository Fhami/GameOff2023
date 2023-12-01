using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

namespace DefaultNamespace
{
    /// <summary>
    /// The base data for a character which is immutable and should not be modified during runtime.
    /// This data is used as a template for creating character instances.
    /// </summary>
    [CreateAssetMenu(menuName = "Gamejam/Character", fileName = "New Character")]
    public class CharacterData : ScriptableObject
    {
        public Character characterPrefab;
        public int health;
        public int startSize;
        public int minSize;
        public int maxSize;
        public bool deathOnMin;
        public bool deathOnMax;
        public int handSize;
        public DeckData deckData;
        [Expandable] public List<FormData> forms;
    }
}