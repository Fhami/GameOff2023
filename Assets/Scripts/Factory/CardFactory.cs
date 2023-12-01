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
            
            return Create(cardData);
        }
        
        public static RuntimeCard Create(CardData cardData)
        {
            // Create new instance of a card.
            RuntimeCard runtimeCard = new RuntimeCard
            {
                cardData = cardData,
                properties = new()
            };

            // Create cards's properties.
            //If you add some thing you also need to edit CloneCard too

            foreach (var propertyKey in PropertyHelper.IntStatusKeys)
            {
                runtimeCard.properties.Add(propertyKey, new Property<int>(0, propertyKey));
            }
            
            runtimeCard.properties.Add(PropertyKey.CARD_STATE, new Property<CardState>(CardState.NONE, PropertyKey.CARD_STATE));

            // Attach card effect modifiers to runtime card
            foreach (EffectData effect in cardData.effects)
            {
                if (effect.effectModifier != null)
                {
                    effect.effectModifier.AttachToRuntimeCard(runtimeCard);
                }
            }
            
            return runtimeCard;
        }

        private const string CardPrefabPath = "Prefabs/Cards/BaseCardPrefab";
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


        public static RuntimeCard CloneCard(RuntimeCard runtimeCard)
        {
            var newCard = Create(runtimeCard.cardData);
            
            foreach (var propertyKey in PropertyHelper.IntStatusKeys)
            {
                runtimeCard.properties.Add(propertyKey, new Property<int>(runtimeCard.properties.Get<int>(propertyKey).Value, propertyKey));
            }
            newCard.properties.Add(PropertyKey.CARD_STATE, new Property<CardState>(CardState.NONE, PropertyKey.CARD_STATE));
            
            return newCard;
        }

        public static CardUI CreateCardUI(CardData cardData)
        {
            //Load and cache card prefab
            var cardUIPrefab = Resources.Load<CardUI>("Prefabs/Cards/CardUIPrefab");

            var newCard = Object.Instantiate(cardUIPrefab);
            newCard.name = cardData.name;
            newCard.InitCard(cardData);

            return newCard;
        }
        
        public static CardUI CreateCardUI(string name)
        {
            return CreateCardUI(Database.cardData[name]);
        }
    }
}