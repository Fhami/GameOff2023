using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using NaughtyAttributes;
using UnityEngine;

namespace DefaultNamespace
{
    [CreateAssetMenu(menuName = "Gamejam/Effect/Gain Shield Effect", fileName = "New Gain Shield Effect")]
    public class GainShieldEffect : EffectData
    {
        [Header("Shield")]
        public ValueSource shieldValueSource;
        
        [ShowIf("shieldValueSource", ValueSource.CARD)]
        public int shieldValue;
        
        [ShowIf("shieldValueSource", ValueSource.CUSTOM)]
        public CustomValueSource customShieldValue;
        
        [ResizableTextArea]
        [ShowIf("shieldValueSource", ValueSource.CUSTOM)]
        public string customShieldDescription;
        
        public override IEnumerator Execute(RuntimeCard card, RuntimeCharacter characterPlayingTheCard, RuntimeCharacter player, RuntimeCharacter cardTarget, List<RuntimeCharacter> enemies)
        {
            // TODO: VFX
            
            int shield = GetEffectValue(card, characterPlayingTheCard, player, cardTarget, enemies);
            
            characterPlayingTheCard.properties.Get<int>(PropertyKey.SHIELD).Value += shield;
            
            yield break;
        }

        public override string GetDescriptionTextWithModifiers(RuntimeCard card, RuntimeCharacter characterPlayingTheCard, RuntimeCharacter player, RuntimeCharacter cardTarget, List<RuntimeCharacter> enemies)
        {
            StringBuilder sb = new();
            
            switch (shieldValueSource)
            {
                case ValueSource.NONE:
                    break;
                case ValueSource.CARD:
                    sb.Append($"Gain {GetEffectValue(card, characterPlayingTheCard, player, cardTarget, enemies).ToString()} shield");
                    break;
                case ValueSource.CUSTOM:
                    sb.Append(" " + customShieldDescription);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            
            sb.Append(".");

            return sb.ToString();
        }

        public override string GetDescriptionText()
        {
            StringBuilder sb = new();
            
            switch (shieldValueSource)
            {
                case ValueSource.NONE:
                    break;
                case ValueSource.CARD:
                    sb.Append($"Gain {GetEffectValue()} shield");
                    break;
                case ValueSource.CUSTOM:
                    sb.Append(" " + customShieldDescription);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            
            sb.Append(".");

            return sb.ToString();
        }
        
        /// <summary>
        /// Get the shield value inside a battle. Calculates the final value with all the modifiers.
        /// </summary>
        public override int GetEffectValue(RuntimeCard card, RuntimeCharacter characterPlayingTheCard, RuntimeCharacter player, RuntimeCharacter cardTarget, List<RuntimeCharacter> enemies)
        {
            int damage = shieldValueSource switch
            {
                ValueSource.NONE => throw new NotSupportedException(),
                ValueSource.CARD => shieldValue,
                ValueSource.CUSTOM => customShieldValue.GetValue(card, characterPlayingTheCard, player, cardTarget, enemies),
                _ => throw new ArgumentOutOfRangeException()
            };
            
            int cardShieldModifier = card.properties.Get<int>(PropertyKey.SHIELD).GetValueWithModifiers(card);
            
            return damage + cardShieldModifier;
        }
        
        /// <summary>
        /// Get shield value outside the battle. If you have a reference to the card instance
        /// the method will also calculate the card upgrades into the final value.
        /// </summary>
        public override string GetEffectValue(RuntimeCard card = null)
        {
            if (card == null)
            {
                return shieldValueSource switch
                {
                    ValueSource.NONE => throw new NotSupportedException(),
                    ValueSource.CARD => shieldValue.ToString(),
                    ValueSource.CUSTOM => "X",
                    _ => throw new ArgumentOutOfRangeException()
                };
            }
          
            return shieldValueSource switch
            {
                ValueSource.NONE => throw new NotSupportedException(),
                ValueSource.CARD => (shieldValue + card.properties.Get<int>(PropertyKey.SHIELD).GetValueWithModifiers(card)).ToString(),
                ValueSource.CUSTOM => "X",
                _ => throw new ArgumentOutOfRangeException()
            };
        }

        public override int GetTimesValue(RuntimeCard card, RuntimeCharacter characterPlayingTheCard, RuntimeCharacter player, RuntimeCharacter cardTarget, List<RuntimeCharacter> enemies)
        {
            return 1;
        }

        public override string GetTimesValue(RuntimeCard card = null)
        {
            return "";
        }
    }
}