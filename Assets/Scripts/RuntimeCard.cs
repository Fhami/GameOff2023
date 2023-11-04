namespace DefaultNamespace
{
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
    }
}