using System;
using System.Collections.Generic;
using NaughtyAttributes;
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

    // TODO: Include more types? We can use the CardType for filtering cards among other things.
    // TODO: See Slay the Spire for reference. We can also show the CardType as a "subtitle" below the card name in the card graphic.
    public enum CardType
    {
        NONE,
        ATTACK,
        DEFEND,
    }
    
    /// <summary>
    /// The base data for a card which is immutable and should not be modified during runtime.
    /// This data is used as a template for creating card instances.
    /// </summary>
    [CreateAssetMenu(menuName = "Gamejam/Card", fileName = "New Card")]
    public class CardData : ScriptableObject
    {
        public CardType cardType;
        public CardDragTarget cardDragTarget;
        [Expandable] public List<EffectData> effects;
    }
}