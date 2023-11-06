namespace DefaultNamespace
{
    /// <summary>
    /// For creating card instances from card data.
    /// </summary>
    public static class CardFactory
    {
        public static RuntimeCard Create(string name)
        {
            // Use the card name to get the card data/template from the database.
            CardData cardData = Database.cardData[name];
            
            // Create new instance of a card.
            RuntimeCard runtimeCard = new RuntimeCard();
            
            runtimeCard.cardData = cardData;

            runtimeCard.properties = new();
            // runtimeCard.properties.Add(PropertyKey.COST, new Property<int>(cardData.cost));
            
            return runtimeCard;
        }
    }
}