using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DefaultNamespace
{
    [CreateAssetMenu(menuName = "Gamejam/Effect/Attack Effect", fileName = "New Attack Effect")]
    public class AttackEffect : EffectData
    {
        public EffectTarget effectTarget;
        public int value;
        
        public override IEnumerator Execute(
            RuntimeCard card,
            RuntimeCharacter characterPlayingTheCard,
            RuntimeCharacter player,
            RuntimeCharacter cardTarget,
            List<RuntimeCharacter> enemies)
        {
            int attackValueWithModifiers = GetCardAttackValueWithModifiers(card, characterPlayingTheCard);

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

            // Process the attack to every target
            foreach (RuntimeCharacter target in targets)
            {
                if (target.properties.Get<int>(PropertyKey.EVADE).Value > 0)
                {
                    yield return Evade(target);
                }
                else
                {
                    yield return Attack(target, attackValueWithModifiers, player, enemies);
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

            return GetDescriptionText(attackValueWithModifiers.ToString());
        }

        public override string GetDescriptionText()
        {
            return GetDescriptionText(value.ToString());
        }

        protected override string GetDescriptionText(string value)
        {
            switch (effectTarget)
            {
                case EffectTarget.NONE: throw new NotSupportedException();
                case EffectTarget.PLAYER: throw new NotSupportedException();
                case EffectTarget.CARD_PLAYER: throw new NotSupportedException();
                case EffectTarget.TARGET:
                {
                    return $"Deal {value} damage.";
                }
                case EffectTarget.ALL_ENEMIES:
                {
                    return $"Deal {value} damage to all enemies.";
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
            yield return BattleManager.OnGameEvent(GameEvent.ON_DEATH, character, player, enemies);
            // TODO: Remove the character from battle (if it's enemy)
        }
        
        private int GetCardAttackValueWithModifiers(RuntimeCard card, RuntimeCharacter player)
        {
            // Card attack value formula: card base attack value + player attack with modifiers + player strength + card attack value modifiers
            int playerAttackWithModifiers = player.properties.Get<int>(PropertyKey.ATTACK).GetValueWithModifiers(player);
            int playerStrength = player.properties.Get<int>(PropertyKey.STRENGTH).GetValueWithModifiers(player);
            int cardAttackWithModifiers = card.properties.Get<int>(PropertyKey.ATTACK).GetValueWithModifiers(card);
            return value + playerAttackWithModifiers + playerStrength + cardAttackWithModifiers;
        }
    }
}