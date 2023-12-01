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
        [Header("Target")]
        public EffectTarget effectTarget;
        
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
            
            List<RuntimeCharacter> targets = new();
            
            // Get the affected targets for the heal effect
            switch (effectTarget)
            {
                case EffectTarget.NONE:
                    break;
                case EffectTarget.PLAYER:
                    targets.Add(player);
                    break;
                case EffectTarget.CARD_PLAYER:
                    targets.Add(characterPlayingTheCard);
                    break;
                case EffectTarget.TARGET:
                    targets.Add(cardTarget);
                    break;
                case EffectTarget.ALL_ENEMIES:
                    targets.AddRange(enemies);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            
            int value = GetEffectValue(card, characterPlayingTheCard, player, cardTarget, enemies);
            var time = GetTimesValue(card, characterPlayingTheCard, player, cardTarget, enemies);

            for (int i = 0; i < time; i++)
            {
                foreach (var target in targets)
                {
                    target.properties.Get<int>(statusEffect).Value += value;
                }
            }
            
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
                    sb.Append($"Gain {GetEffectValue(card, characterPlayingTheCard, player, cardTarget, enemies).ToString()} <color={Colors.COLOR_STATUS}>{statusEffect.ToString()}</color>");
                    break;
                case ValueSource.CUSTOM:
                    sb.Append(" " + customDescription);
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
            
            switch (valueSource)
            {
                case ValueSource.NONE:
                    break;
                case ValueSource.CARD:
                    sb.Append($"Gain {GetEffectValue()} <color={Colors.COLOR_STATUS}>{statusEffect.ToString()}</color>");
                    break;
                case ValueSource.CUSTOM:
                    sb.Append(" " + customDescription);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            
            switch (timesValueSource)
            {
                case ValueSource.NONE:
                    break;
                case ValueSource.CARD:
                    sb.Append($" {GetTimesValue()} times");
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
    }
}