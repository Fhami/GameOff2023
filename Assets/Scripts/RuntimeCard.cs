namespace DefaultNamespace
{
    public enum CardState
    {
        NONE,
        PLAYING,
        DRAW_PILE,
        HAND,
        DISCARD_PILE,
        FADED,
        DESTROYED
    }
    
    /// <summary>
    /// The runtime instance of a card. This can be modified during runtime.
    /// </summary>
    public class RuntimeCard
    {
        /// <summary>
        /// The base data of the card.
        /// </summary>
        public CardData cardData;
        
        /// <summary>
        /// The properties/stats of the card.
        /// </summary>
        public PropertyContainer properties;

        public CardState cardState;
    }
}