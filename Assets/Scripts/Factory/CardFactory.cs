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
            runtimeCard.properties.Add(PropertyKey.ATTACK, new Property<int>(0, PropertyKey.ATTACK));
            runtimeCard.properties.Add(PropertyKey.SHIELD, new Property<int>(0, PropertyKey.SHIELD));
            runtimeCard.properties.Add(PropertyKey.SHIELD_UP, new Property<int>(0, PropertyKey.SHIELD_UP));
            runtimeCard.properties.Add(PropertyKey.SHIELD_DOWN, new Property<int>(0, PropertyKey.SHIELD_DOWN));
            runtimeCard.properties.Add(PropertyKey.HEAL, new Property<int>(0, PropertyKey.HEAL));
            runtimeCard.properties.Add(PropertyKey.SIZE, new Property<int>(0, PropertyKey.SIZE));
            runtimeCard.properties.Add(PropertyKey.STABLE, new Property<int>(0, PropertyKey.STABLE));
            runtimeCard.properties.Add(PropertyKey.THORNS, new Property<int>(0, PropertyKey.THORNS));
            runtimeCard.properties.Add(PropertyKey.TIMES, new Property<int>(0, PropertyKey.TIMES));
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
            
            newCard.properties.Add(PropertyKey.ATTACK, new Property<int>(runtimeCard.properties.Get<int>(PropertyKey.ATTACK).Value, PropertyKey.ATTACK));
            newCard.properties.Add(PropertyKey.SHIELD, new Property<int>(runtimeCard.properties.Get<int>(PropertyKey.SHIELD).Value, PropertyKey.SHIELD));
            newCard.properties.Add(PropertyKey.SHIELD_UP, new Property<int>(runtimeCard.properties.Get<int>(PropertyKey.SHIELD_UP).Value, PropertyKey.SHIELD_UP));
            newCard.properties.Add(PropertyKey.SHIELD_DOWN, new Property<int>(runtimeCard.properties.Get<int>(PropertyKey.SHIELD_DOWN).Value, PropertyKey.SHIELD_DOWN));
            newCard.properties.Add(PropertyKey.HEAL, new Property<int>(runtimeCard.properties.Get<int>(PropertyKey.HEAL).Value, PropertyKey.HEAL));
            newCard.properties.Add(PropertyKey.SIZE, new Property<int>(runtimeCard.properties.Get<int>(PropertyKey.SIZE).Value, PropertyKey.SIZE));
            newCard.properties.Add(PropertyKey.STABLE, new Property<int>(runtimeCard.properties.Get<int>(PropertyKey.STABLE).Value, PropertyKey.STABLE));
            newCard.properties.Add(PropertyKey.TIMES, new Property<int>(runtimeCard.properties.Get<int>(PropertyKey.TIMES).Value, PropertyKey.TIMES));
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