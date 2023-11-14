using System;
using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

namespace DefaultNamespace
{
    public enum ValueSource
    {
        NONE,
        CARD,
        CUSTOM
    }
    
    [CreateAssetMenu(menuName = "Gamejam/Effect/Attack Effect", fileName = "New Attack Effect")]
    public class AttackEffect : EffectData
    {
        [Header("The target(s) the damage get applied to")]
        public EffectTarget effectTarget;
        
        [Header("The source of the X value in 'deal X damage'")]
        public ValueSource damageValueSource;
        
        [ShowIf("damageValueSource", ValueSource.CARD)]
        public int damageValue;
        
        [ShowIf("damageValueSource", ValueSource.CUSTOM)]
        public DamageValueSource customDamageValue;
        
        [Header("The source of the X value in 'deal 3 damage X times'")]
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

            int attackValueWithModifiers = GetCardAttackValueWithModifiers(card, characterPlayingTheCard);
            int times = GetTimesValue();
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
                        yield return Attack(target, attackValueWithModifiers, player, enemies);
                    }
                }
            }
            
            // Clear the attacker's strength stack after the attack
            characterPlayingTheCard.properties.Get<int>(PropertyKey.STRENGTH).Value = 0;
        }
        
        public override string GetDescriptionText(RuntimeCard card, RuntimeCharacter playerCharacter)
        {
            // TODO: You can use rich text here to change the ATK value color in the card like in
            // TODO: slay the spire if the ATK is modified (you can just compare calculatedDamage with the base card damage)
            int attackValueWithModifiers = GetCardAttackValueWithModifiers(card, playerCharacter);

            switch (effectTarget)
            {
                case EffectTarget.NONE: throw new NotSupportedException();
                case EffectTarget.PLAYER: throw new NotSupportedException();
                case EffectTarget.CARD_PLAYER: throw new NotSupportedException();
                case EffectTarget.TARGET:
                {
                    return $"Deal {attackValueWithModifiers.ToString()} damage.";
                }
                case EffectTarget.ALL_ENEMIES:
                {
                    return $"Deal {attackValueWithModifiers.ToString()} damage to all enemies.";
                }
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private IEnumerator Evade(RuntimeCharacter target)
        {
            // TODO: VFX, animation etc.
            target.properties.Get<int>(PropertyKey.EVADE).Value -= 1;
            yield break;
        }
        
        private IEnumerator Attack(RuntimeCharacter target, int incomingAttack, RuntimeCharacter player, List<RuntimeCharacter> enemies)
        {
            // TODO: VFX, animation etc.
            Property<int> shield = target.properties.Get<int>(PropertyKey.SHIELD);
            Property<int> health = target.properties.Get<int>(PropertyKey.HEALTH);
            Property<int> maxHealth = target.properties.Get<int>(PropertyKey.MAX_HEALTH);

            // Keep track of how much health the target had before receiving damage
            int healthBefore = health.Value;
            
            // Calculate the attack value after shield absorption (i.e. reduce shield value from attack value)
            int attackAbsorbedByShield = Mathf.Min(incomingAttack, shield.Value);
            int attack = incomingAttack - attackAbsorbedByShield;
                    
            // Reduce the absorbed attack value from the shield
            shield.Value = Mathf.Max(shield.Value - attackAbsorbedByShield, 0);

            // Reduce the final attack value from the target's health
            health.Value = Mathf.Clamp(health.Value - attack, 0, maxHealth.GetValueWithModifiers(target));

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
        
        private int GetCardAttackValueWithModifiers(RuntimeCard card, RuntimeCharacter player)
        {
            // Card attack value formula: card base attack value + player attack with modifiers + player strength + card attack value modifiers
            int playerAttackWithModifiers = player.properties.Get<int>(PropertyKey.ATTACK).GetValueWithModifiers(player);
            int playerStrength = player.properties.Get<int>(PropertyKey.STRENGTH).GetValueWithModifiers(player);
            int cardAttackWithModifiers = card.properties.Get<int>(PropertyKey.ATTACK).GetValueWithModifiers(card);
            return GetDamageValue() + playerAttackWithModifiers + playerStrength + cardAttackWithModifiers;
        }

        private int GetDamageValue()
        {
            return damageValueSource switch
            {
                ValueSource.NONE => throw new NotSupportedException(),
                ValueSource.CARD => damageValue,
                ValueSource.CUSTOM => throw new NotImplementedException("TODO"), // TODO: This will be customDamageValue.GetValue();
                _ => throw new ArgumentOutOfRangeException()
            };
        }
        
        private int GetTimesValue()
        {
            return timesValueSource switch
            {
                ValueSource.NONE => 1,
                ValueSource.CARD => timesValue,
                ValueSource.CUSTOM => throw new NotImplementedException("TODO"), // TODO: This will be customTimesValue.GetValue();
                _ => throw new ArgumentOutOfRangeException()
            };
        }
    }
}