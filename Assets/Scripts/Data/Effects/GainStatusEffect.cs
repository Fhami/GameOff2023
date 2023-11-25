using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using NaughtyAttributes;
using RoboRyanTron.SearchableEnum;
using UnityEngine;

namespace DefaultNamespace
{
    [CreateAssetMenu(menuName = "Gamejam/Effect/GainStatusEffect")]
    public class GainStatusEffect : EffectData
    {
        [SearchableEnum]
        public PropertyKey statusEffect;
        
        [Header("Value")]
        public ValueSource valueSource;
        
        [ShowIf("valueSource", ValueSource.CARD)]
        public int value;
        
        [ShowIf("valueSource", ValueSource.CUSTOM)]
        public CustomValueSource customValue;
        
        [ResizableTextArea]
        [ShowIf("valueSource", ValueSource.CUSTOM)]
        public string customDescription;
        
        public override IEnumerator Execute(RuntimeCard card, RuntimeCharacter characterPlayingTheCard, RuntimeCharacter player, RuntimeCharacter cardTarget, List<RuntimeCharacter> enemies)
        {
            // TODO: VFX
            
            int value = GetEffectValue(card, characterPlayingTheCard, player, cardTarget, enemies);
            
            characterPlayingTheCard.properties.Get<int>(statusEffect).Value += value;
            
            yield break;
        }

        public override string GetDescriptionTextWithModifiers(RuntimeCard card, RuntimeCharacter characterPlayingTheCard, RuntimeCharacter player, RuntimeCharacter cardTarget, List<RuntimeCharacter> enemies)
        {
            StringBuilder sb = new();
            
            switch (valueSource)
            {
                case ValueSource.NONE:
                    break;
                case ValueSource.CARD:
                    sb.Append($"Gain {GetEffectValue(card, characterPlayingTheCard, player, cardTarget, enemies).ToString()} {statusEffect.ToString()}");
                    break;
                case ValueSource.CUSTOM:
                    sb.Append(" " + customDescription);
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
            
            switch (valueSource)
            {
                case ValueSource.NONE:
                    break;
                case ValueSource.CARD:
                    sb.Append($"Gain {GetEffectValue()} {statusEffect.ToString()}");
                    break;
                case ValueSource.CUSTOM:
                    sb.Append(" " + customDescription);
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
        public override int GetEffectValue(RuntimeCard card, RuntimeCharacter characterPlayingTheCard, RuntimeCharacter player, RuntimeCharacter cardTarget, List<RuntimeCharacter> enemies)
        {
            int damage = valueSource switch
            {
                ValueSource.NONE => throw new NotSupportedException(),
                ValueSource.CARD => value,
                ValueSource.CUSTOM => customValue.GetValue(card, characterPlayingTheCard, player, cardTarget, enemies),
                _ => throw new ArgumentOutOfRangeException()
            };
            
            int cardStatusModifier = card.properties.Get<int>(statusEffect).GetValueWithModifiers(card);
            
            return damage + cardStatusModifier;
        }
        
        /// <summary>
        /// Get thorns value outside the battle. If you have a reference to the card instance
        /// the method will also calculate the card upgrades into the final value.
        /// </summary>
        public override string GetEffectValue(RuntimeCard card = null)
        {
            if (card == null)
            {
                return valueSource switch
                {
                    ValueSource.NONE => throw new NotSupportedException(),
                    ValueSource.CARD => value.ToString(),
                    ValueSource.CUSTOM => "X",
                    _ => throw new ArgumentOutOfRangeException()
                };
            }
          
            return valueSource switch
            {
                ValueSource.NONE => throw new NotSupportedException(),
                ValueSource.CARD => (value + card.properties.Get<int>(statusEffect).GetValueWithModifiers(card)).ToString(),
                ValueSource.CUSTOM => "X",
                _ => throw new ArgumentOutOfRangeException()
            };
        }

        public override int GetTimesValue(RuntimeCard card, RuntimeCharacter characterPlayingTheCard, RuntimeCharacter player,
            RuntimeCharacter cardTarget, List<RuntimeCharacter> enemies)
        {
            return 1;
        }

        public override string GetTimesValue(RuntimeCard card = null)
        {
            return "1";
        }
    }
}