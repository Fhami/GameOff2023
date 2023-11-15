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
    public class RuntimeCard : RuntimeEntity
    {
        /// <summary>
        /// The base data of the card.
        /// </summary>
        public CardData cardData;

        public CardState cardState;

        public Card Card;
    }
}