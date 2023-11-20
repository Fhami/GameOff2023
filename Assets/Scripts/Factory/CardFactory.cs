using UnityEngine;

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
            RuntimeCard runtimeCard = new RuntimeCard
            {
                cardData = cardData,
                properties = new()
            };

            // Create cards's properties.
            runtimeCard.properties.Add(PropertyKey.ATTACK, new Property<int>(0));
            runtimeCard.properties.Add(PropertyKey.SHIELD, new Property<int>(0));
            runtimeCard.properties.Add(PropertyKey.SIZE, new Property<int>(0));
            runtimeCard.properties.Add(PropertyKey.TIMES, new Property<int>(0));
            runtimeCard.properties.Add(PropertyKey.CARD_STATE, new Property<CardState>(CardState.NONE));

            return runtimeCard;
        }

        private const string CardPrefabPath = "CardPrefab";
        private static Card cardPrefab;
        
        /// <summary>
        /// Instantiate card prefab and update card visual from cardData
        /// </summary>
        public static Card CreateCardObject(RuntimeCard runtimeCard)
        {
            //Load and cache card prefab
            if (!cardPrefab)
                cardPrefab = Resources.Load<Card>(CardPrefabPath);

            var newCard = Object.Instantiate(cardPrefab);
            newCard.name = runtimeCard.cardData.name;
            newCard.InitCard(runtimeCard);

            return newCard;
        }

        public static Card CreateCardObject(string name)
        {
            return CreateCardObject(Create(name));
        }

        public static CardUI CreateCardUI(RuntimeCard runtimeCard)
        {
            //Load and cache card prefab
            var cardUIPrefab = Resources.Load<CardUI>("Temp/CardUIMockup");

            var newCard = Object.Instantiate(cardUIPrefab);
            newCard.name = runtimeCard.cardData.name;
            newCard.InitCard(runtimeCard);

            return newCard;
        }
        public static CardUI CreateCardUI(string name)
        {
            return CreateCardUI(Create(name));
        }
    }
}