namespace DefaultNamespace
{
    /// <summary>
    /// For creating card instances from card data.
    /// </summary>
    public static class CardFactory
    {
        public static RuntimeCard Create(string name)
        {
            CardData cardData = Database.cardData[name];
            
            RuntimeCard runtimeCard = new RuntimeCard();

            runtimeCard.cardData = cardData;
            
            runtimeCard.properties = new();
            runtimeCard.properties.Add(PropertyKey.COST, new Property<int>(1));
            
            return runtimeCard;
        }
    }
}