using System;
using System.Collections.Generic;
using UnityEngine;

namespace DefaultNamespace
{
    /// <summary>
    /// Where can we drag and drop the card? Does it need to be dropped on enemy,
    /// player or is dropping it to the background enough?
    /// </summary>
    [Flags]
    public enum CardDragTarget
    {
        NONE = 0,
        ENEMY = 1,
        PLAYER = 1 << 1,
        BACKGROUND = 1 << 2,
    }
    
    /// <summary>
    /// The base data for a card which is immutable and should not be modified during runtime.
    /// This data is used as a template for creating card instances.
    /// </summary>
    [CreateAssetMenu(menuName = "Gamejam/Card", fileName = "New Card")]
    public class CardData : ScriptableObject
    {
        public bool fade;
        public bool destroy;
        public CardDragTarget cardDragTarget;
        public List<Effect> effects;
        public List<EffectData> effectsV2;
    }
}