using System;
using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

namespace DefaultNamespace
{
    public enum ValueState
    {
        NORMAL,
        INCREASED,
        DECREASED
    }
    
    public abstract class EffectData : ScriptableObject
    {
        public IntentData intent;

        public EffectModifier effectModifier;
        
        [Header("Times")]
        public ValueSource timesValueSource;
        
        [ShowIf("timesValueSource", ValueSource.CARD)]
        public int timesValue;

        [ShowIf("timesValueSource", ValueSource.CUSTOM)]
        public CustomValueSource customTimesValue;
        
        [ResizableTextArea]
        [ShowIf("timesValueSource", ValueSource.CUSTOM)]
        public string customTimesDescription;
        
        /// <summary>
        /// A coroutine for playing a single card. Executes card's effects and can also trigger different skills.
        /// </summary>
        /// <param name="card">The card being played.</param>
        /// <param name="characterPlayingTheCard">The character who is playing the card.</param>
        /// <param name="player">The player character (human).</param>
        /// <param name="cardTarget">The target character. Can be either player or enemy.</param>
        /// <param name="enemies">List of all enemies in the current battle. Can be used for AOE etc.</param>
        public abstract IEnumerator Execute(
            RuntimeCard card, 
            RuntimeCharacter characterPlayingTheCard,
            RuntimeCharacter player, 
            RuntimeCharacter cardTarget, 
            List<RuntimeCharacter> enemies);
        
        /// <summary>
        /// Get the effect text with modifiers for the card front. This should be used in battle.
        /// </summary>
        public abstract string GetDescriptionTextWithModifiers(RuntimeCard card,
            RuntimeCharacter characterPlayingTheCard,
            RuntimeCharacter player,
            RuntimeCharacter cardTarget,
            List<RuntimeCharacter> enemies);
        
        /// <summary>
        /// Get the effect text for the card front. This should be used outside the battle.
        /// </summary>
        /// <returns></returns>
        public abstract string GetDescriptionText();

        /// <summary>
        /// Get the effect value inside a battle. Calculates the final value with all the modifiers.
        /// </summary>
        public abstract int GetEffectValue(
            RuntimeCard card,
            RuntimeCharacter characterPlayingTheCard,
            RuntimeCharacter player,
            RuntimeCharacter cardTarget,
            List<RuntimeCharacter> enemies,
            out ValueState valueState);
        
        /// <summary>
        /// Get effect value outside the battle. If you have a reference to the card instance
        /// the method will also calculate the card upgrades into the final value.
        /// </summary>
        public abstract string GetEffectValue(RuntimeCard card = null);

        /// <summary>
        /// Get the times value inside a battle. Calculates the final value with all the modifiers.
        /// </summary>
        public int GetTimesValue(
            RuntimeCard card,
            RuntimeCharacter characterPlayingTheCard,
            RuntimeCharacter player,
            RuntimeCharacter cardTarget,
            List<RuntimeCharacter> enemies)
        {
            int times = timesValueSource switch
            {
                ValueSource.NONE => 1,
                ValueSource.CARD => timesValue,
                ValueSource.CUSTOM => customTimesValue.GetValue(card, characterPlayingTheCard, player, cardTarget, enemies),
                _ => throw new ArgumentOutOfRangeException()
            };

            int cardTimesModifier = card.properties.Get<int>(PropertyKey.TIMES).GetValueWithModifiers(characterPlayingTheCard);
            
            return times + cardTimesModifier;
        }
        
        /// <summary>
        /// Get times value outside the battle. If you have a reference to the card instance
        /// the method will also calculate the card upgrades into the final value.
        /// </summary>
        public string GetTimesValue(RuntimeCard card = null)
        {
            if (card == null)
            {
                return timesValueSource switch
                {
                    ValueSource.NONE => 1.ToString(),
                    ValueSource.CARD => timesValue.ToString(),
                    ValueSource.CUSTOM => "X",
                    _ => throw new ArgumentOutOfRangeException()
                };
            }
          
            return timesValueSource switch
            {
                ValueSource.NONE => 1.ToString(),
                ValueSource.CARD => (timesValue + card.properties.Get<int>(PropertyKey.TIMES).GetValueWithModifiers(card)).ToString(),
                ValueSource.CUSTOM => "X",
                _ => throw new ArgumentOutOfRangeException()
            };
        }

        public virtual void PreviewEffect(RuntimeCard card, RuntimeCharacter characterPlayingTheCard, RuntimeCharacter player, RuntimeCharacter cardTarget, List<RuntimeCharacter> enemies)
        {
            
        }
    }
}