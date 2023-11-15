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
        [Header("The target(s) the damage get applied to")]
        public EffectTarget effectTarget;
        
        [Header("The source of the 'damage' value")]
        public ValueSource damageValueSource;
        
        [ShowIf("damageValueSource", ValueSource.CARD)]
        public int damageValue;
        
        [ShowIf("damageValueSource", ValueSource.CUSTOM)]
        public DamageValueSource customDamageValue;
        
        [Header("The source of the 'times' value")]
        public ValueSource timesValueSource;
        
        [ShowIf("timesValueSource", ValueSource.CARD)]
        public int timesValue;

        [ShowIf("timesValueSource", ValueSource.CUSTOM)]
        public TimesValueSource customTimesValue;
        
        public override IEnumerator Execute(
            RuntimeCard card,
            RuntimeCharacter characterPlayingTheCard,
            RuntimeCharacter player,
            RuntimeCharacter cardTarget,
            List<RuntimeCharacter> enemies)
        {
            List<RuntimeCharacter> targets = new();
            
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

            int damageValueWithModifiers = GetDamageValue(card, characterPlayingTheCard, player, cardTarget, enemies);
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
                        yield return Attack(target, damageValueWithModifiers, player, enemies);
                    }
                }
            }
            
            // Clear the attacker's strength stack after the attack
            characterPlayingTheCard.properties.Get<int>(PropertyKey.STRENGTH).Value = 0;
        }
        
        public override string GetDescriptionTextWithModifiers(
            RuntimeCard card,
            RuntimeCharacter characterPlayingTheCard,
            RuntimeCharacter player,
            RuntimeCharacter cardTarget,
            List<RuntimeCharacter> enemies)
        {
            StringBuilder sb = new();

            // Step 1) Build "Deal X damage" / "Deal X damage to all enemies" / "Deal damage equal to your.." string
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
                            sb.Append(customDamageValue.GetDescription());
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
                            sb.Append(customDamageValue.GetDescription());
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            
            // Step 2) Build " X times." / " number of times you've discarded cards this turn" string
            switch (timesValueSource)
            {
                case ValueSource.NONE:
                    break;
                case ValueSource.CARD:
                    sb.Append($" {GetTimesValue(card, characterPlayingTheCard, player, cardTarget, enemies).ToString()} times.");
                    break;
                case ValueSource.CUSTOM:
                    sb.Append(customTimesValue.GetDescription());
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

            // Step 1) Build "Deal X damage" / "Deal X damage to all enemies" / "Deal damage equal to your.." string
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
                            sb.Append(customDamageValue.GetDescription());
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
                            sb.Append(customDamageValue.GetDescription());
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            
            // Step 2) Build " X times." / " number of times you've discarded cards this turn" string
            switch (timesValueSource)
            {
                case ValueSource.NONE:
                    break;
                case ValueSource.CARD:
                    sb.Append($" {GetTimesValue()} times.");
                    break;
                case ValueSource.CUSTOM:
                    sb.Append(customTimesValue.GetDescription());
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
            // TODO: VFX, animation etc.
            character.properties.Get<CharacterState>(PropertyKey.CHARACTER_STATE).Value = CharacterState.DEAD;
            yield return BattleManager.OnGameEvent(GameEvent.ON_DEATH, character, player, enemies);
            // TODO: Remove the character from battle (if it's enemy)
        }
        
        private int GetDamageValue(RuntimeCard card,
            RuntimeCharacter characterPlayingTheCard,
            RuntimeCharacter player,
            RuntimeCharacter cardTarget,
            List<RuntimeCharacter> enemies)
        {
            int value = damageValueSource switch
            {
                ValueSource.NONE => throw new NotSupportedException(),
                ValueSource.CARD => damageValue,
                ValueSource.CUSTOM => customDamageValue.GetValue(card, characterPlayingTheCard, player, cardTarget, enemies),
                _ => throw new ArgumentOutOfRangeException()
            };
            
            int playerAttackWithModifiers = player.properties.Get<int>(PropertyKey.ATTACK).GetValueWithModifiers(player);
            int playerStrength = player.properties.Get<int>(PropertyKey.STRENGTH).GetValueWithModifiers(player);
            int cardAttackWithModifiers = card.properties.Get<int>(PropertyKey.ATTACK).GetValueWithModifiers(card);
            
            return value + playerAttackWithModifiers + playerStrength + cardAttackWithModifiers;
        }
        
        private int GetTimesValue(RuntimeCard card,
            RuntimeCharacter characterPlayingTheCard,
            RuntimeCharacter player,
            RuntimeCharacter cardTarget,
            List<RuntimeCharacter> enemies)
        {
            int value = timesValueSource switch
            {
                ValueSource.NONE => 1,
                ValueSource.CARD => timesValue,
                ValueSource.CUSTOM => customTimesValue.GetValue(card, characterPlayingTheCard, player, cardTarget, enemies),
                _ => throw new ArgumentOutOfRangeException()
            };

            return value;
        }
        
        /// <summary>
        /// Get damage value outside the battle. If card is null we don't have card upgrades calculated in the value.
        /// </summary>
        private string GetDamageValue(RuntimeCard card = null)
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
        /// Get times value outside the battle. If card is null we don't have card upgrades calculated in the value.
        /// </summary>
        private string GetTimesValue(RuntimeCard card = null)
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