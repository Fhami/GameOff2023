using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using NaughtyAttributes;
using UnityEngine;

namespace DefaultNamespace
{
    [CreateAssetMenu(menuName = "Gamejam/Effect/Heal Effect", fileName = "New Heal Effect")]
    public class HealEffect : EffectData
    {
        [Header("Target")]
        public EffectTarget effectTarget;
        
        [Header("Heal")]
        public ValueSource healValueSource;
        
        [ShowIf("healValueSource", ValueSource.CARD)]
        public int healValue;
        
        [ShowIf("healValueSource", ValueSource.CUSTOM)]
        public CustomValueSource customHealValue;
        
        [ResizableTextArea]
        [ShowIf("healValueSource", ValueSource.CUSTOM)]
        public string customHealDescription;
        
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
                    throw new NotSupportedException();
                case EffectTarget.CARD_PLAYER:
                    targets.Add(characterPlayingTheCard);
                    break;
                case EffectTarget.TARGET:
                    throw new NotSupportedException();
                case EffectTarget.ALL_ENEMIES:
                    targets.AddRange(enemies);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            
            int heal = GetEffectValue(card, characterPlayingTheCard, player, cardTarget, enemies, out _);
            var time = GetTimesValue(card, characterPlayingTheCard, player, cardTarget, enemies);

            for (int i = 0; i < time; i++)
            {
                foreach (RuntimeCharacter target in targets)
                {
                    Property<int> health = target.properties.Get<int>(PropertyKey.HEALTH);
                    Property<int> maxHealth = target.properties.Get<int>(PropertyKey.MAX_HEALTH);
            
                    // Keep track of how much health we had before healing
                    int healthBefore = health.Value;
            
                    // Apply the healing value
                    health.Value = Mathf.Clamp(health.Value + heal, 0, maxHealth.GetValueWithModifiers(target));

                    if (healthBefore != health.Value)
                    {
                        yield return BattleManager.current.OnGameEvent(GameEvent.ON_HEALTH_CHANGED, target, player, enemies);
                    }
                }
            }
        }

        public override string GetDescriptionTextWithModifiers(RuntimeCard card, RuntimeCharacter characterPlayingTheCard, RuntimeCharacter player, RuntimeCharacter cardTarget, List<RuntimeCharacter> enemies)
        {
            StringBuilder sb = new();

            switch (effectTarget)
            {
                case EffectTarget.NONE:
                    throw new NotSupportedException();
                case EffectTarget.PLAYER:
                    throw new NotImplementedException();
                case EffectTarget.CARD_PLAYER:
                {
                    switch (healValueSource)
                    {
                        case ValueSource.NONE:
                            break;
                        case ValueSource.CARD:
                            int value = GetEffectValue(card, characterPlayingTheCard, player, cardTarget, enemies, out ValueState valueState);
                            sb.Append($"Heal <color={Colors.GetNumberColor(valueState)}>{value.ToString()}</color>");
                            break;
                        case ValueSource.CUSTOM:
                            sb.Append(" " + customHealDescription);
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                    break;
                }
                case EffectTarget.TARGET:
                    break;
                case EffectTarget.ALL_ENEMIES:
                {
                    switch (healValueSource)
                    {
                        case ValueSource.NONE:
                            break;
                        case ValueSource.CARD:
                            int value = GetEffectValue(card, characterPlayingTheCard, player, cardTarget, enemies, out ValueState valueState);
                            sb.Append($"Heal all enemies by <color={Colors.GetNumberColor(valueState)}>{value.ToString()}</color>");
                            break;
                        case ValueSource.CUSTOM:
                            sb.Append(" " + customHealDescription);
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                    break;
                }
                default:
                    throw new ArgumentOutOfRangeException();
            }
            
            sb.Append(".");

            return sb.ToString();
        }

        public override string GetDescriptionText()
        {
            StringBuilder sb = new();

            switch (effectTarget)
            {
                case EffectTarget.NONE:
                    throw new NotSupportedException();
                case EffectTarget.PLAYER:
                    throw new NotImplementedException();
                case EffectTarget.CARD_PLAYER:
                {
                    switch (healValueSource)
                    {
                        case ValueSource.NONE:
                            break;
                        case ValueSource.CARD:
                            sb.Append($"Heal {GetEffectValue()}");
                            break;
                        case ValueSource.CUSTOM:
                            sb.Append(" " + customHealDescription);
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                    break;
                }
                case EffectTarget.TARGET:
                    break;
                case EffectTarget.ALL_ENEMIES:
                {
                    switch (healValueSource)
                    {
                        case ValueSource.NONE:
                            break;
                        case ValueSource.CARD:
                            sb.Append($"Heal all enemies by {GetEffectValue()}");
                            break;
                        case ValueSource.CUSTOM:
                            sb.Append(" " + customHealDescription);
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                    break;
                }
                default:
                    throw new ArgumentOutOfRangeException();
            }
            
            sb.Append(".");

            return sb.ToString();
        }
        
        /// <summary>
        /// Get the heal value inside a battle. Calculates the final value with all the modifiers.
        /// </summary>
        public override int GetEffectValue(
            RuntimeCard card,
            RuntimeCharacter characterPlayingTheCard,
            RuntimeCharacter player,
            RuntimeCharacter cardTarget,
            List<RuntimeCharacter> enemies,
            out ValueState valueState)
        {
            int value = healValueSource switch
            {
                ValueSource.NONE => throw new NotSupportedException(),
                ValueSource.CARD => healValue,
                ValueSource.CUSTOM => customHealValue.GetValue(card, characterPlayingTheCard, player, cardTarget, enemies),
                _ => throw new ArgumentOutOfRangeException()
            };
            
            int modifiers = card.properties.Get<int>(PropertyKey.HEAL).GetValueWithModifiers(card);

            int valueWithModifiers = value + modifiers;
            
            valueState = ValueState.NORMAL;
            if (valueWithModifiers > value)
            {
                valueState = ValueState.INCREASED;
            }
            else if (valueWithModifiers < value)
            {
                valueState = ValueState.DECREASED;
            }
            
            return valueWithModifiers;
        }
        
        /// <summary>
        /// Get heal value outside the battle. If you have a reference to the card instance
        /// the method will also calculate the card upgrades into the final value.
        /// </summary>
        public override string GetEffectValue(RuntimeCard card = null)
        {
            if (card == null)
            {
                return healValueSource switch
                {
                    ValueSource.NONE => throw new NotSupportedException(),
                    ValueSource.CARD => healValue.ToString(),
                    ValueSource.CUSTOM => "X",
                    _ => throw new ArgumentOutOfRangeException()
                };
            }
          
            return healValueSource switch
            {
                ValueSource.NONE => throw new NotSupportedException(),
                ValueSource.CARD => (healValue + card.properties.Get<int>(PropertyKey.HEAL).GetValueWithModifiers(card)).ToString(),
                ValueSource.CUSTOM => "X",
                _ => throw new ArgumentOutOfRangeException()
            };
        }
    }
}