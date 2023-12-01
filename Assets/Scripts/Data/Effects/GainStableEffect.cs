using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using NaughtyAttributes;
using UnityEngine;

namespace DefaultNamespace
{
    [CreateAssetMenu(menuName = "Gamejam/Effect/Gain Stable Effect", fileName = "New Gain Stable Effect")]
    public class GainStableEffect : EffectData
    {
        [Header("Stable")]
        public ValueSource stableValueSource;
        
        [ShowIf("stableValueSource", ValueSource.CARD)]
        public int stableValue;
        
        [ShowIf("stableValueSource", ValueSource.CUSTOM)]
        public CustomValueSource customStableValue;
        
        [ResizableTextArea]
        [ShowIf("stableValueSource", ValueSource.CUSTOM)]
        public string customStableDescription;
        
        public override IEnumerator Execute(RuntimeCard card, RuntimeCharacter characterPlayingTheCard, RuntimeCharacter player, RuntimeCharacter cardTarget, List<RuntimeCharacter> enemies)
        {
            int stable = GetEffectValue(card, characterPlayingTheCard, player, cardTarget, enemies, out _);
            var time = GetTimesValue(card, characterPlayingTheCard, player, cardTarget, enemies);
            
            characterPlayingTheCard.properties.Get<int>(PropertyKey.STABLE).Value += stable + time;
            
            yield break;
        }

        public override string GetDescriptionTextWithModifiers(RuntimeCard card, RuntimeCharacter characterPlayingTheCard, RuntimeCharacter player, RuntimeCharacter cardTarget, List<RuntimeCharacter> enemies)
        {
            StringBuilder sb = new();
            
            switch (stableValueSource)
            {
                case ValueSource.NONE:
                    break;
                case ValueSource.CARD:
                    int value = GetEffectValue(card, characterPlayingTheCard, player, cardTarget, enemies, out ValueState valueState);
                    sb.Append($"Gain <color={Colors.GetNumberColor(valueState)}>{value.ToString()}</color> <color={Colors.COLOR_STATUS}>stable</color>");
                    break;
                case ValueSource.CUSTOM:
                    sb.Append(" " + customStableDescription);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            
            switch (timesValueSource)
            {
                case ValueSource.NONE:
                    break;
                case ValueSource.CARD:
                    sb.Append($" {GetTimesValue(card, characterPlayingTheCard, player, cardTarget, enemies).ToString()} times");
                    break;
                case ValueSource.CUSTOM:
                    sb.Append(" " + customTimesDescription);
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
            
            switch (stableValueSource)
            {
                case ValueSource.NONE:
                    break;
                case ValueSource.CARD:
                    sb.Append($"Gain {GetEffectValue()} stable");
                    break;
                case ValueSource.CUSTOM:
                    sb.Append(" " + customStableDescription);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            
            switch (timesValueSource)
            {
                case ValueSource.NONE:
                    break;
                case ValueSource.CARD:
                    sb.Append($" {GetTimesValue().ToString()} times");
                    break;
                case ValueSource.CUSTOM:
                    sb.Append(" " + customTimesDescription);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            
            sb.Append(".");

            return sb.ToString();
        }
        
        /// <summary>
        /// Get the stable value inside a battle. Calculates the final value with all the modifiers.
        /// </summary>
        public override int GetEffectValue(
            RuntimeCard card,
            RuntimeCharacter characterPlayingTheCard,
            RuntimeCharacter player,
            RuntimeCharacter cardTarget,
            List<RuntimeCharacter> enemies,
            out ValueState valueState)
        {
            int stable = stableValueSource switch
            {
                ValueSource.NONE => throw new NotSupportedException(),
                ValueSource.CARD => stableValue,
                ValueSource.CUSTOM => customStableValue.GetValue(card, characterPlayingTheCard, player, cardTarget, enemies),
                _ => throw new ArgumentOutOfRangeException()
            };
            
            int modifiers = card.properties.Get<int>(PropertyKey.STABLE).GetValueWithModifiers(card);

            int stableWithModifiers = stable + modifiers;
            
            valueState = ValueState.NORMAL;
            if (stableWithModifiers > stable)
            {
                valueState = ValueState.INCREASED;
            }
            else if (stableWithModifiers < stable)
            {
                valueState = ValueState.DECREASED;
            }
            
            return stableWithModifiers;
        }
        
        /// <summary>
        /// Get stable value outside the battle. If you have a reference to the card instance
        /// the method will also calculate the card upgrades into the final value.
        /// </summary>
        public override string GetEffectValue(RuntimeCard card = null)
        {
            if (card == null)
            {
                return stableValueSource switch
                {
                    ValueSource.NONE => throw new NotSupportedException(),
                    ValueSource.CARD => stableValue.ToString(),
                    ValueSource.CUSTOM => "X",
                    _ => throw new ArgumentOutOfRangeException()
                };
            }
          
            return stableValueSource switch
            {
                ValueSource.NONE => throw new NotSupportedException(),
                ValueSource.CARD => (stableValue + card.properties.Get<int>(PropertyKey.STABLE).GetValueWithModifiers(card)).ToString(),
                ValueSource.CUSTOM => "X",
                _ => throw new ArgumentOutOfRangeException()
            };
        }
    }
}