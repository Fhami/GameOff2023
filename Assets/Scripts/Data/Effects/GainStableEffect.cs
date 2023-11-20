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
            // TODO: VFX
            
            int stable = GetEffectValue(card, characterPlayingTheCard, player, cardTarget, enemies);
            
            characterPlayingTheCard.properties.Get<int>(PropertyKey.STABLE).Value += stable;
            
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
                    sb.Append($"Gain {GetEffectValue(card, characterPlayingTheCard, player, cardTarget, enemies).ToString()} stable");
                    break;
                case ValueSource.CUSTOM:
                    sb.Append(" " + customStableDescription);
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
            
            sb.Append(".");

            return sb.ToString();
        }
        
        /// <summary>
        /// Get the stable value inside a battle. Calculates the final value with all the modifiers.
        /// </summary>
        public override int GetEffectValue(RuntimeCard card, RuntimeCharacter characterPlayingTheCard, RuntimeCharacter player, RuntimeCharacter cardTarget, List<RuntimeCharacter> enemies)
        {
            int damage = stableValueSource switch
            {
                ValueSource.NONE => throw new NotSupportedException(),
                ValueSource.CARD => stableValue,
                ValueSource.CUSTOM => customStableValue.GetValue(card, characterPlayingTheCard, player, cardTarget, enemies),
                _ => throw new ArgumentOutOfRangeException()
            };
            
            int cardStableModifier = card.properties.Get<int>(PropertyKey.STABLE).GetValueWithModifiers(card);
            
            return damage + cardStableModifier;
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

        public override int GetTimesValue(RuntimeCard card, RuntimeCharacter characterPlayingTheCard, RuntimeCharacter player,
            RuntimeCharacter cardTarget, List<RuntimeCharacter> enemies)
        {
            throw new NotImplementedException();
        }

        public override string GetTimesValue(RuntimeCard card = null)
        {
            throw new NotImplementedException();
        }
    }
}