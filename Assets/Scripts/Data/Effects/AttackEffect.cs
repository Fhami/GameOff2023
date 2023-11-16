using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using NaughtyAttributes;
using UnityEngine;

namespace DefaultNamespace
{
    [CreateAssetMenu(menuName = "Gamejam/Effect/Attack Effect", fileName = "New Attack Effect")]
    public class AttackEffect : EffectData
    {
        [Header("Target")]
        public EffectTarget effectTarget;
        
        [Header("Damage")]
        public ValueSource damageValueSource;
        
        [ShowIf("damageValueSource", ValueSource.CARD)]
        public int damageValue;
        
        [ShowIf("damageValueSource", ValueSource.CUSTOM)]
        public CustomValueSource customDamageValue;
        
        [ResizableTextArea]
        [ShowIf("damageValueSource", ValueSource.CUSTOM)]
        public string customDamageDescription;
        
        [Header("Times")]
        public ValueSource timesValueSource;
        
        [ShowIf("timesValueSource", ValueSource.CARD)]
        public int timesValue;

        [ShowIf("timesValueSource", ValueSource.CUSTOM)]
        public CustomValueSource customTimesValue;
        
        [ResizableTextArea]
        [ShowIf("timesValueSource", ValueSource.CUSTOM)]
        public string customTimesDescription;
        
        public override IEnumerator Execute(RuntimeCard card, RuntimeCharacter characterPlayingTheCard, RuntimeCharacter player, RuntimeCharacter cardTarget, List<RuntimeCharacter> enemies)
        {
            List<RuntimeCharacter> targets = new();
            
            // Get the affected targets for the size change effect
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

            // Process the attack effect to every target
            int damage = GetDamageValue(card, characterPlayingTheCard, player, cardTarget, enemies);
            int times = GetTimesValue(card, characterPlayingTheCard, player, cardTarget, enemies);
            
            for (int i = 0; i < times; i++)
            {
                // Process the attack to every target
                foreach (RuntimeCharacter target in targets)
                {
                    if (target.properties.Get<int>(PropertyKey.EVADE).Value > 0)
                    {
                        // NOTE: Based on the current logic each damage in multi-damage (e.g 3x5 dmg) effect will reduce one evade. So 3x5 dmg would reduce 3 evade.
                        // NOTE: If we want 1 evade to evade the whole attack, we need to remove the target with evade from the list of targets or something..
                        yield return Evade(target);
                    }
                    // A target might die during a multi-damage effect, so let's make sure we only attack targets that are ALIVE
                    else if (target.properties.Get<CharacterState>(PropertyKey.CHARACTER_STATE).Value == CharacterState.ALIVE)
                    {
                        yield return Attack(target, damage, player, enemies);
                    }
                }
            }
            
            // Clear the attacker's strength stack after the attack
            characterPlayingTheCard.properties.Get<int>(PropertyKey.STRENGTH).Value = 0;
        }
        
        public override string GetDescriptionTextWithModifiers(RuntimeCard card, RuntimeCharacter characterPlayingTheCard, RuntimeCharacter player, RuntimeCharacter cardTarget, List<RuntimeCharacter> enemies)
        {
            StringBuilder sb = new();

            switch (effectTarget)
            {
                case EffectTarget.NONE:
                    throw new NotSupportedException();
                case EffectTarget.PLAYER:
                    throw new NotSupportedException();
                case EffectTarget.CARD_PLAYER:
                    throw new NotSupportedException();
                case EffectTarget.TARGET:
                    switch (damageValueSource)
                    {
                        case ValueSource.NONE:
                            throw new NotSupportedException();
                        case ValueSource.CARD:
                            sb.Append($"Deal {GetDamageValue(card, characterPlayingTheCard, player, cardTarget, enemies).ToString()} damage");
                            break;
                        case ValueSource.CUSTOM:
                            sb.Append(customDamageDescription);
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                    break;
                case EffectTarget.ALL_ENEMIES:
                    switch (damageValueSource)
                    {
                        case ValueSource.NONE:
                            throw new NotSupportedException();
                        case ValueSource.CARD:
                            sb.Append($"Deal {GetDamageValue(card, characterPlayingTheCard, player, cardTarget, enemies).ToString()} damage to all enemies");
                            break;
                        case ValueSource.CUSTOM:
                            sb.Append(customDamageDescription);
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            
            switch (timesValueSource)
            {
                case ValueSource.NONE:
                    sb.Append(".");
                    break;
                case ValueSource.CARD:
                    sb.Append($" {GetTimesValue(card, characterPlayingTheCard, player, cardTarget, enemies).ToString()} times.");
                    break;
                case ValueSource.CUSTOM:
                    sb.Append(customTimesDescription);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

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
                    throw new NotSupportedException();
                case EffectTarget.CARD_PLAYER:
                    throw new NotSupportedException();
                case EffectTarget.TARGET:
                    switch (damageValueSource)
                    {
                        case ValueSource.NONE:
                            throw new NotSupportedException();
                        case ValueSource.CARD:
                            sb.Append($"Deal {GetDamageValue()} damage");
                            break;
                        case ValueSource.CUSTOM:
                            sb.Append(customDamageDescription);
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                    break;
                case EffectTarget.ALL_ENEMIES:
                    switch (damageValueSource)
                    {
                        case ValueSource.NONE:
                            throw new NotSupportedException();
                        case ValueSource.CARD:
                            sb.Append($"Deal {GetDamageValue()} damage to all enemies");
                            break;
                        case ValueSource.CUSTOM:
                            sb.Append(customDamageDescription);
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            
            switch (timesValueSource)
            {
                case ValueSource.NONE:
                    sb.Append(".");
                    break;
                case ValueSource.CARD:
                    sb.Append($" {GetTimesValue()} times.");
                    break;
                case ValueSource.CUSTOM:
                    sb.Append(customTimesDescription);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return sb.ToString();
        }

        private IEnumerator Evade(RuntimeCharacter target)
        {
            // TODO: VFX, animation etc.
            
            target.properties.Get<int>(PropertyKey.EVADE).Value -= 1;
            yield break;
        }
        
        private IEnumerator Attack(RuntimeCharacter target, int incomingDamage, RuntimeCharacter player, List<RuntimeCharacter> enemies)
        {
            // TODO: VFX, animation etc.
            
            Property<int> shield = target.properties.Get<int>(PropertyKey.SHIELD);
            Property<int> health = target.properties.Get<int>(PropertyKey.HEALTH);
            Property<int> maxHealth = target.properties.Get<int>(PropertyKey.MAX_HEALTH);

            // Keep track of how much health the target had before receiving damage
            int healthBefore = health.Value;
            
            // Calculate the attack value after shield absorption (i.e. reduce shield value from attack value)
            int damageAbsorbedByShield = Mathf.Min(incomingDamage, shield.Value);
            int damage = incomingDamage - damageAbsorbedByShield;
                    
            // Reduce the absorbed attack value from the shield
            shield.Value = Mathf.Max(shield.Value - damageAbsorbedByShield, 0);

            // Reduce the final attack value from the target's health
            health.Value = Mathf.Clamp(health.Value - damage, 0, maxHealth.GetValueWithModifiers(target));

            if (health.Value > 0)
            {
                if (healthBefore != health.Value)
                {
                    // If the target didn't die but their health changed -> trigger ON_HEALTH_CHANGED game event
                    yield return BattleManager.OnGameEvent(GameEvent.ON_HEALTH_CHANGED, target, player, enemies);
                }
            }
            else
            {
                yield return Kill(target, player, enemies);
            }
        }

        private static IEnumerator Kill(RuntimeCharacter character, RuntimeCharacter player, List<RuntimeCharacter> enemies)
        {
            // TODO: VFX, animation etc. Remove the character from battle (if it's enemy)
            
            character.properties.Get<CharacterState>(PropertyKey.CHARACTER_STATE).Value = CharacterState.DEAD;
            yield return BattleManager.OnGameEvent(GameEvent.ON_DEATH, character, player, enemies);
        }
        
        /// <summary>
        /// Get the damage value inside a battle. Calculates the final value with all the modifiers.
        /// </summary>
        public int GetDamageValue(RuntimeCard card, RuntimeCharacter characterPlayingTheCard, RuntimeCharacter player, RuntimeCharacter cardTarget, List<RuntimeCharacter> enemies)
        {
            int damage = damageValueSource switch
            {
                ValueSource.NONE => throw new NotSupportedException(),
                ValueSource.CARD => damageValue,
                ValueSource.CUSTOM => customDamageValue.GetValue(card, characterPlayingTheCard, player, cardTarget, enemies),
                _ => throw new ArgumentOutOfRangeException()
            };
            
            int playerAttackModifier = player.properties.Get<int>(PropertyKey.ATTACK).GetValueWithModifiers(player);
            int playerStrengthModifier = player.properties.Get<int>(PropertyKey.STRENGTH).GetValueWithModifiers(player);
            int cardAttackModifier = card.properties.Get<int>(PropertyKey.ATTACK).GetValueWithModifiers(card);
            
            return damage + playerAttackModifier + playerStrengthModifier + cardAttackModifier;
        }
        
        /// <summary>
        /// Get the times value inside a battle. Calculates the final value with all the modifiers.
        /// </summary>
        public int GetTimesValue(RuntimeCard card, RuntimeCharacter characterPlayingTheCard, RuntimeCharacter player, RuntimeCharacter cardTarget, List<RuntimeCharacter> enemies)
        {
            int times = timesValueSource switch
            {
                ValueSource.NONE => 1,
                ValueSource.CARD => timesValue,
                ValueSource.CUSTOM => customTimesValue.GetValue(card, characterPlayingTheCard, player, cardTarget, enemies),
                _ => throw new ArgumentOutOfRangeException()
            };

            int cardTimesModifier = card.properties.Get<int>(PropertyKey.TIMES).GetValueWithModifiers(card);
            
            return times + cardTimesModifier;
        }
        
        /// <summary>
        /// Get damage value outside the battle. If you have a reference to the card instance
        /// the method will also calculate the card upgrades into the final value.
        /// </summary>
        public string GetDamageValue(RuntimeCard card = null)
        {
            if (card == null)
            {
                return damageValueSource switch
                {
                    ValueSource.NONE => throw new NotSupportedException(),
                    ValueSource.CARD => damageValue.ToString(),
                    ValueSource.CUSTOM => "X",
                    _ => throw new ArgumentOutOfRangeException()
                };
            }
          
            return damageValueSource switch
            {
                ValueSource.NONE => throw new NotSupportedException(),
                ValueSource.CARD => (damageValue + card.properties.Get<int>(PropertyKey.ATTACK).GetValueWithModifiers(card)).ToString(),
                ValueSource.CUSTOM => "X",
                _ => throw new ArgumentOutOfRangeException()
            };
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
                ValueSource.CARD => (timesValue + card.properties.Get<int>(PropertyKey.ATTACK).GetValueWithModifiers(card)).ToString(),
                ValueSource.CUSTOM => "X",
                _ => throw new ArgumentOutOfRangeException()
            };
        }
    }
}