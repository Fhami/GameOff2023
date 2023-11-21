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
            int damage = GetEffectValue(card, characterPlayingTheCard, player, cardTarget, enemies);
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
                        yield return Attack(target, damage, characterPlayingTheCard, player, cardTarget, enemies);
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
                            sb.Append($"Deal {GetEffectValue(card, characterPlayingTheCard, player, cardTarget, enemies).ToString()} damage");
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
                            sb.Append($"Deal {GetEffectValue(card, characterPlayingTheCard, player, cardTarget, enemies).ToString()} damage to all enemies");
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
                            sb.Append($"Deal {GetEffectValue()} damage");
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
                            sb.Append($"Deal {GetEffectValue()} damage to all enemies");
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

        private IEnumerator Evade(RuntimeCharacter target)
        {
            // TODO: VFX, animation etc.
            target.Character.PlayParticle(ParticleKey.EVADE);
            
            target.properties.Get<int>(PropertyKey.EVADE).Value -= 1;
            yield break;
        }
        
        private IEnumerator Attack(RuntimeCharacter target, int incomingDamage, RuntimeCharacter characterPlayingTheCard, RuntimeCharacter player, RuntimeCharacter cardTarget, List<RuntimeCharacter> enemies)
        {
            Property<int> shield = target.properties.Get<int>(PropertyKey.SHIELD);
            Property<int> health = target.properties.Get<int>(PropertyKey.HEALTH);
            Property<int> maxHealth = target.properties.Get<int>(PropertyKey.MAX_HEALTH);

            // Keep track of how much health the target had before receiving damage
            int healthBefore = health.Value;
            
            // Calculate the attack value after shield absorption (i.e. reduce shield value from attack value)
            int damageAbsorbedByShield = Mathf.Min(incomingDamage, shield.Value);
            int damage = incomingDamage - damageAbsorbedByShield;

            //Play animation/vfx
            yield return characterPlayingTheCard.Character.PlayAttackFeedback(target.Character.FrontPos);

            // Reduce the absorbed attack value from the shield
            shield.Value = Mathf.Max(shield.Value - damageAbsorbedByShield, 0);

            // Reduce the final attack value from the target's health
            health.Value = Mathf.Clamp(health.Value - damage, 0, maxHealth.GetValueWithModifiers(target));

            // If target has thorns, deal damage from thorns to the attacker
            int thornsDamage = target.properties.Get<int>(PropertyKey.THORNS).GetValueWithModifiers(target);
            if (thornsDamage > 0)
            {
                yield return Attack(characterPlayingTheCard, thornsDamage, target, player, characterPlayingTheCard, enemies);
            }
            
            if (health.Value > 0)
            {
                if (healthBefore != health.Value)
                {
                    // If the target didn't die but their health changed -> trigger ON_HEALTH_CHANGED game event
                    yield return BattleManager.current.OnGameEvent(GameEvent.ON_HEALTH_CHANGED, target, player, enemies);
                    
                    //Damaged animation/vfx will be bind with properties.OnChanged
                }
            }
            else
            {
                yield return BattleManager.current.Kill(target, characterPlayingTheCard, player, cardTarget, enemies);
            }
        }

        /// <summary>
        /// Get the damage value inside a battle. Calculates the final value with all the modifiers.
        /// </summary>
        public override int GetEffectValue(RuntimeCard card, RuntimeCharacter characterPlayingTheCard, RuntimeCharacter player, RuntimeCharacter cardTarget, List<RuntimeCharacter> enemies)
        {
            int damage = damageValueSource switch
            {
                ValueSource.NONE => throw new NotSupportedException(),
                ValueSource.CARD => damageValue,
                ValueSource.CUSTOM => customDamageValue.GetValue(card, characterPlayingTheCard, player, cardTarget, enemies),
                _ => throw new ArgumentOutOfRangeException()
            };
            
            int playerAttackModifier = characterPlayingTheCard.properties.Get<int>(PropertyKey.ATTACK).GetValueWithModifiers(characterPlayingTheCard);
            int playerStrengthModifier = characterPlayingTheCard.properties.Get<int>(PropertyKey.STRENGTH).GetValueWithModifiers(characterPlayingTheCard);
            int cardAttackModifier = card.properties.Get<int>(PropertyKey.ATTACK).GetValueWithModifiers(characterPlayingTheCard);
            
            return damage + playerAttackModifier + playerStrengthModifier + cardAttackModifier;
        }
        
        /// <summary>
        /// Get the times value inside a battle. Calculates the final value with all the modifiers.
        /// </summary>
        public override int GetTimesValue(RuntimeCard card, RuntimeCharacter characterPlayingTheCard, RuntimeCharacter player, RuntimeCharacter cardTarget, List<RuntimeCharacter> enemies)
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
        /// Get damage value outside the battle. If you have a reference to the card instance
        /// the method will also calculate the card upgrades into the final value.
        /// </summary>
        public override string GetEffectValue(RuntimeCard card = null)
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
        public override string GetTimesValue(RuntimeCard card = null)
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