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
            runtimeCard.properties.Add(PropertyKey.STRENGTH, new Property<int>(0, PropertyKey.STRENGTH));
            runtimeCard.properties.Add(PropertyKey.EVASION, new Property<int>(0, PropertyKey.EVASION));
            runtimeCard.properties.Add(PropertyKey.STUN, new Property<int>(0, PropertyKey.STUN));
            runtimeCard.properties.Add(PropertyKey.STABLE, new Property<int>(0, PropertyKey.STABLE));
            runtimeCard.properties.Add(PropertyKey.THORNS, new Property<int>(0, PropertyKey.THORNS));
            runtimeCard.properties.Add(PropertyKey.DECAY, new Property<int>(0, PropertyKey.DECAY));
            runtimeCard.properties.Add(PropertyKey.GROW, new Property<int>(0, PropertyKey.GROW));
            runtimeCard.properties.Add(PropertyKey.UNSTABLE, new Property<int>(0, PropertyKey.UNSTABLE));
            runtimeCard.properties.Add(PropertyKey.ARMOR, new Property<int>(0, PropertyKey.ARMOR));
            runtimeCard.properties.Add(PropertyKey.SHIELD_DOWN, new Property<int>(0, PropertyKey.SHIELD_DOWN));
            runtimeCard.properties.Add(PropertyKey.SHIELD_UP, new Property<int>(0, PropertyKey.SHIELD_UP));
            runtimeCard.properties.Add(PropertyKey.ENVENOMED, new Property<int>(0, PropertyKey.ENVENOMED));
            runtimeCard.properties.Add(PropertyKey.FRAGILE, new Property<int>(0, PropertyKey.FRAGILE));
            runtimeCard.properties.Add(PropertyKey.INTEGRITY, new Property<int>(0, PropertyKey.INTEGRITY));
            runtimeCard.properties.Add(PropertyKey.NUTRIENT, new Property<int>(0, PropertyKey.NUTRIENT));
            runtimeCard.properties.Add(PropertyKey.PROTECTION, new Property<int>(0, PropertyKey.PROTECTION));
            runtimeCard.properties.Add(PropertyKey.REGEN, new Property<int>(0, PropertyKey.REGEN));
            runtimeCard.properties.Add(PropertyKey.REPEL, new Property<int>(0, PropertyKey.REPEL));
            runtimeCard.properties.Add(PropertyKey.STABLE, new Property<int>(0, PropertyKey.STABLE));
            runtimeCard.properties.Add(PropertyKey.STRENGTH_DOWN, new Property<int>(0, PropertyKey.STRENGTH_DOWN));
            runtimeCard.properties.Add(PropertyKey.VULNERABLE, new Property<int>(0, PropertyKey.VULNERABLE));
            runtimeCard.properties.Add(PropertyKey.WEAK, new Property<int>(0, PropertyKey.WEAK));
            runtimeCard.properties.Add(PropertyKey.HEAL, new Property<int>(0, PropertyKey.HEAL));
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
            newCard.properties.Add(PropertyKey.STRENGTH, new Property<int>(runtimeCard.properties.Get<int>(PropertyKey.STRENGTH).Value, PropertyKey.STRENGTH));
            newCard.properties.Add(PropertyKey.EVASION, new Property<int>(runtimeCard.properties.Get<int>(PropertyKey.EVASION).Value, PropertyKey.EVASION));
            newCard.properties.Add(PropertyKey.STUN, new Property<int>(runtimeCard.properties.Get<int>(PropertyKey.STUN).Value, PropertyKey.STUN));
            newCard.properties.Add(PropertyKey.STABLE, new Property<int>(runtimeCard.properties.Get<int>(PropertyKey.STABLE).Value, PropertyKey.STABLE));
            newCard.properties.Add(PropertyKey.THORNS, new Property<int>(runtimeCard.properties.Get<int>(PropertyKey.THORNS).Value, PropertyKey.THORNS));
            newCard.properties.Add(PropertyKey.DECAY, new Property<int>(runtimeCard.properties.Get<int>(PropertyKey.DECAY).Value, PropertyKey.DECAY));
            newCard.properties.Add(PropertyKey.GROW, new Property<int>(runtimeCard.properties.Get<int>(PropertyKey.GROW).Value, PropertyKey.GROW));
            newCard.properties.Add(PropertyKey.UNSTABLE, new Property<int>(runtimeCard.properties.Get<int>(PropertyKey.UNSTABLE).Value, PropertyKey.UNSTABLE));
            newCard.properties.Add(PropertyKey.ARMOR, new Property<int>(runtimeCard.properties.Get<int>(PropertyKey.ARMOR).Value, PropertyKey.ARMOR));
            newCard.properties.Add(PropertyKey.SHIELD_DOWN, new Property<int>(runtimeCard.properties.Get<int>(PropertyKey.SHIELD_DOWN).Value, PropertyKey.SHIELD_DOWN));
            newCard.properties.Add(PropertyKey.SHIELD_UP, new Property<int>(runtimeCard.properties.Get<int>(PropertyKey.SHIELD_UP).Value, PropertyKey.SHIELD_UP));
            newCard.properties.Add(PropertyKey.ENVENOMED, new Property<int>(runtimeCard.properties.Get<int>(PropertyKey.ENVENOMED).Value, PropertyKey.ENVENOMED));
            newCard.properties.Add(PropertyKey.FRAGILE, new Property<int>(runtimeCard.properties.Get<int>(PropertyKey.FRAGILE).Value, PropertyKey.FRAGILE));
            newCard.properties.Add(PropertyKey.INTEGRITY, new Property<int>(runtimeCard.properties.Get<int>(PropertyKey.INTEGRITY).Value, PropertyKey.INTEGRITY));
            newCard.properties.Add(PropertyKey.NUTRIENT, new Property<int>(runtimeCard.properties.Get<int>(PropertyKey.NUTRIENT).Value, PropertyKey.NUTRIENT));
            newCard.properties.Add(PropertyKey.PROTECTION, new Property<int>(runtimeCard.properties.Get<int>(PropertyKey.PROTECTION).Value, PropertyKey.PROTECTION));
            newCard.properties.Add(PropertyKey.REGEN, new Property<int>(runtimeCard.properties.Get<int>(PropertyKey.REGEN).Value, PropertyKey.REGEN));
            newCard.properties.Add(PropertyKey.REPEL, new Property<int>(runtimeCard.properties.Get<int>(PropertyKey.REPEL).Value, PropertyKey.REPEL));
            newCard.properties.Add(PropertyKey.STABLE, new Property<int>(runtimeCard.properties.Get<int>(PropertyKey.STABLE).Value, PropertyKey.STABLE));
            newCard.properties.Add(PropertyKey.STRENGTH_DOWN, new Property<int>(runtimeCard.properties.Get<int>(PropertyKey.STRENGTH_DOWN).Value, PropertyKey.STRENGTH_DOWN));
            newCard.properties.Add(PropertyKey.VULNERABLE, new Property<int>(runtimeCard.properties.Get<int>(PropertyKey.VULNERABLE).Value, PropertyKey.VULNERABLE));
            newCard.properties.Add(PropertyKey.WEAK, new Property<int>(runtimeCard.properties.Get<int>(PropertyKey.WEAK).Value, PropertyKey.WEAK));
            newCard.properties.Add(PropertyKey.HEAL, new Property<int>(runtimeCard.properties.Get<int>(PropertyKey.HEAL).Value, PropertyKey.HEAL));
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