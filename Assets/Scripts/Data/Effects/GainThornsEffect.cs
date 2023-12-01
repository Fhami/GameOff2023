using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using NaughtyAttributes;
using UnityEngine;

namespace DefaultNamespace
{
    [CreateAssetMenu(menuName = "Gamejam/Effect/Gain Thorns Effect", fileName = "New Gain Thorns Effect")]
    [Obsolete("Use GainStatusEffect")]
    public class GainThornsEffect : EffectData
    {
        [Header("Thorns")]
        public ValueSource thornsValueSource;
        
        [ShowIf("thornsValueSource", ValueSource.CARD)]
        public int thornsValue;
        
        [ShowIf("thornsValueSource", ValueSource.CUSTOM)]
        public CustomValueSource customThornsValue;
        
        [ResizableTextArea]
        [ShowIf("thornsValueSource", ValueSource.CUSTOM)]
        public string customThornsDescription;
        
        public override IEnumerator Execute(RuntimeCard card, RuntimeCharacter characterPlayingTheCard, RuntimeCharacter player, RuntimeCharacter cardTarget, List<RuntimeCharacter> enemies)
        {
            int thorns = GetEffectValue(card, characterPlayingTheCard, player, cardTarget, enemies, out _);
            
            characterPlayingTheCard.properties.Get<int>(PropertyKey.THORNS).Value += thorns;
            
            yield break;
        }

        public override string GetDescriptionTextWithModifiers(RuntimeCard card, RuntimeCharacter characterPlayingTheCard, RuntimeCharacter player, RuntimeCharacter cardTarget, List<RuntimeCharacter> enemies)
        {
            StringBuilder sb = new();
            
            switch (thornsValueSource)
            {
                case ValueSource.NONE:
                    break;
                case ValueSource.CARD:
                    int value = GetEffectValue(card, characterPlayingTheCard, player, cardTarget, enemies, out ValueState valueState);
                    sb.Append($"Gain <color={Colors.GetNumberColor(valueState)}>{value.ToString()}</color> <color={Colors.COLOR_STATUS}>thorns</color>");
                    break;
                case ValueSource.CUSTOM:
                    sb.Append(" " + customThornsDescription);
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
            
            switch (thornsValueSource)
            {
                case ValueSource.NONE:
                    break;
                case ValueSource.CARD:
                    sb.Append($"Gain {GetEffectValue()} <color={Colors.COLOR_STATUS}>thorns</color>");
                    break;
                case ValueSource.CUSTOM:
                    sb.Append(" " + customThornsDescription);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            
            sb.Append(".");

            return sb.ToString();
        }
        
        /// <summary>
        /// Get the thorns value inside a battle. Calculates the final value with all the modifiers.
        /// </summary>
        public override int GetEffectValue(
            RuntimeCard card,
            RuntimeCharacter characterPlayingTheCard,
            RuntimeCharacter player,
            RuntimeCharacter cardTarget,
            List<RuntimeCharacter> enemies,
            out ValueState valueState)
        {
            int damage = thornsValueSource switch
            {
                ValueSource.NONE => throw new NotSupportedException(),
                ValueSource.CARD => thornsValue,
                ValueSource.CUSTOM => customThornsValue.GetValue(card, characterPlayingTheCard, player, cardTarget, enemies),
                _ => throw new ArgumentOutOfRangeException()
            };
            
            int modifiers = card.properties.Get<int>(PropertyKey.THORNS).GetValueWithModifiers(card);

            int damageWithModifiers = damage + modifiers;
            
            valueState = ValueState.NORMAL;
            if (damageWithModifiers > damage)
            {
                valueState = ValueState.INCREASED;
            }
            else if (damageWithModifiers < damage)
            {
                valueState = ValueState.DECREASED;
            }
            
            return damageWithModifiers;
        }
        
        /// <summary>
        /// Get thorns value outside the battle. If you have a reference to the card instance
        /// the method will also calculate the card upgrades into the final value.
        /// </summary>
        public override string GetEffectValue(RuntimeCard card = null)
        {
            if (card == null)
            {
                return thornsValueSource switch
                {
                    ValueSource.NONE => throw new NotSupportedException(),
                    ValueSource.CARD => thornsValue.ToString(),
                    ValueSource.CUSTOM => "X",
                    _ => throw new ArgumentOutOfRangeException()
                };
            }
          
            return thornsValueSource switch
            {
                ValueSource.NONE => throw new NotSupportedException(),
                ValueSource.CARD => (thornsValue + card.properties.Get<int>(PropertyKey.THORNS).GetValueWithModifiers(card)).ToString(),
                ValueSource.CUSTOM => "X",
                _ => throw new ArgumentOutOfRangeException()
            };
        }
    }
}