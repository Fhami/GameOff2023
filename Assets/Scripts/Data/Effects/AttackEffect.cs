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
            RuntimeCharacter playerCharacter,
            RuntimeCharacter targetCharacter,
            List<RuntimeCharacter> enemyCharacters)
        {
            // TODO: VFX

            int attackValueWithModifiers = GetAttackValueWithModifiers(card, characterPlayingTheCard);
            
            if (effectTarget == EffectTarget.ALL_ENEMIES) // Handle AOE attack to multiple targets
            {
                foreach (RuntimeCharacter enemyCharacter in enemyCharacters)
                {
                    Property<int> evade = enemyCharacter.properties.Get<int>(PropertyKey.EVADE);
                    
                    if (evade.Value > 0)
                    {
                        // TODO: Handle EVADE logic and VFX
                        evade.Value--;
                    }
                    else
                    {
                        Attack(enemyCharacter, attackValueWithModifiers);
                    }
                }
            }
            else if (effectTarget == EffectTarget.TARGET) // Handle attack to single target
            {
                Property<int> evade = targetCharacter.properties.Get<int>(PropertyKey.EVADE);
                
                if (evade.Value > 0)
                {
                    // TODO: Handle EVADE logic and VFX
                    evade.Value--;
                }
                else
                {
                    Attack(targetCharacter, attackValueWithModifiers);
                }
            }
            else
            {
                throw new NotImplementedException("For now attack effect only supports TARGET and ALL_ENEMIES.");
            }
            
            // Clear character's strength stack after the attack
            characterPlayingTheCard.properties.Get<int>(PropertyKey.STRENGTH).Value = 0;
            
            yield break;
        }

        public override string GetDescriptionText(RuntimeCard card, RuntimeCharacter playerCharacter)
        {
            // TODO: You can use rich text here to change the ATK value color in the card like in
            // TODO: slay the spire if the ATK is modified (you can just compare calculatedDamage with the base card damage)
            int attackValueWithModifiers = GetAttackValueWithModifiers(card, playerCharacter);

            switch (effectTarget)
            {
                case EffectTarget.NONE: throw new NotSupportedException();
                case EffectTarget.PLAYER: throw new NotSupportedException();
                case EffectTarget.CARD_PLAYER: throw new NotSupportedException();
                case EffectTarget.TARGET:
                {
                    return $"Deal {attackValueWithModifiers} damage.";
                }
                case EffectTarget.ALL_ENEMIES:
                {
                    return $"Deal {attackValueWithModifiers} damage to all enemies.";
                }
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private int GetAttackValueWithModifiers(RuntimeCard card, RuntimeCharacter player)
        {
            // Card attack value formula: card base attack value + player attack with modifiers + player strength + card attack value modifiers
            int playerAttackWithModifiers = player.properties.Get<int>(PropertyKey.ATTACK).GetValueWithModifiers(player);
            int playerStrength = player.properties.Get<int>(PropertyKey.STRENGTH).GetValueWithModifiers(player);
            int cardAttackWithModifiers = card.properties.Get<int>(PropertyKey.ATTACK).GetValueWithModifiers(card);
            return value + playerAttackWithModifiers + playerStrength + cardAttackWithModifiers;
        }

        private void Attack(RuntimeCharacter target, int incomingAttack)
        {
            Property<int> shield = target.properties.Get<int>(PropertyKey.SHIELD);
            Property<int> size = target.properties.Get<int>(PropertyKey.SIZE);
            Property<int> maxSize = target.properties.Get<int>(PropertyKey.MAX_SIZE);

            // Calculate the attack after shield absorption
            int attackAbsorbedByShield = Mathf.Min(incomingAttack, shield.Value);
            int attack = incomingAttack - attackAbsorbedByShield;
                    
            // Reduce the target's shield by the attack absorbed
            shield.Value = Mathf.Max(shield.Value - attackAbsorbedByShield, 0);

            FormData formBeforeAttack = target.GetCurrentForm();

            // Apply the attack value by reducing target's size
            size.Value = Mathf.Clamp(size.Value - attack, 0, maxSize.GetValueWithModifiers(target));
            
            FormData formAfterAttack = target.GetCurrentForm();

            if (formBeforeAttack != formAfterAttack)
            {
                target.properties.Get<int>(PropertyKey.FORM_CHANGED_COUNT_CURRENT_TURN).Value++;
                target.properties.Get<int>(PropertyKey.ENEMY_ATTACK_PATTERN_CARD_INDEX).Value = 0;
            }
        }
    }
}