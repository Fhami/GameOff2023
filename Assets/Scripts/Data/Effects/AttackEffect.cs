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
        public int damage;
        
        public override IEnumerator Execute(
            RuntimeCard card,
            RuntimeCharacter characterPlayingTheCard,
            RuntimeCharacter playerCharacter,
            RuntimeCharacter targetCharacter,
            List<RuntimeCharacter> enemyCharacters)
        {
            // TODO: VFX

            // Calculate the damage with modifiers (card damage + modifiers from character who is playing the card)
            int incomingDamage = GetDamageWithModifiers(card, characterPlayingTheCard);
            
            if (effectTarget == EffectTarget.ALL_ENEMIES) // Handle AOE damage to multiple targets
            {
                foreach (RuntimeCharacter enemyCharacter in enemyCharacters)
                {
                    Property<int> evade = enemyCharacter.properties.Get<int>(PropertyKey.EVADE);
                    
                    if (evade.Value > 0) // Handle evade logic
                    {
                        // TODO: VFX / animation for evade
                        evade.Value--;
                    }
                    else
                    {
                        DealDamage(enemyCharacter, incomingDamage);
                    }
                }
            }
            else if (effectTarget == EffectTarget.TARGET) // Handle damage to single target
            {
                Property<int> evade = targetCharacter.properties.Get<int>(PropertyKey.EVADE);
                
                if (evade.Value > 0) // Handle evade logic
                {
                    // TODO: VFX / animation for evade
                    evade.Value--;
                }
                else
                {
                    DealDamage(targetCharacter, incomingDamage);
                }
            }
            else
            {
                throw new NotImplementedException("For now attack effect only supports TARGET and ALL_ENEMIES.");
            }
            
            // Clear the strength stack after the attack
            characterPlayingTheCard.properties.Get<int>(PropertyKey.STRENGTH).Value = 0;
            
            yield break;
        }

        public override string GetDescriptionText(RuntimeCard card, RuntimeCharacter playerCharacter)
        {
            // TODO: You can use rich text here to change the ATK value color in the card like in
            // TODO: slay the spire if the ATK is modified (you can just compare calculatedDamage with the base card damage)
            int damageWithModifiers = GetDamageWithModifiers(card, playerCharacter);

            switch (effectTarget)
            {
                case EffectTarget.NONE: throw new NotSupportedException();
                case EffectTarget.PLAYER: throw new NotSupportedException();
                case EffectTarget.CARD_PLAYER: throw new NotSupportedException();
                case EffectTarget.TARGET:
                {
                    return $"Deal {damageWithModifiers} damage.";
                }
                case EffectTarget.ALL_ENEMIES:
                {
                    return $"Deal {damageWithModifiers} damage to all enemies.";
                }
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private int GetDamageWithModifiers(RuntimeCard card, RuntimeCharacter player)
        {
            // Card damage formula: damage + player attack + player strength + card attack
            int playerAttack = player.properties.Get<int>(PropertyKey.ATTACK).GetValueWithModifiers(player);
            int playerPowerUp = player.properties.Get<int>(PropertyKey.STRENGTH).GetValueWithModifiers(player);
            int cardAttack = card.properties.Get<int>(PropertyKey.ATTACK).GetValueWithModifiers(card);
            return damage + playerAttack + playerPowerUp + cardAttack;
        }

        private void DealDamage(RuntimeCharacter target, int incomingDamage)
        {
            Property<int> targetShield = target.properties.Get<int>(PropertyKey.SHIELD);
            Property<int> targetHealth = target.properties.Get<int>(PropertyKey.HEALTH);
                    
            // Calculate the damage after shield absorption
            int damageAbsorbedByShield = Mathf.Min(incomingDamage, targetShield.Value);
            int damageToTarget = incomingDamage - damageAbsorbedByShield;
                    
            // Reduce the target's shield by the damage absorbed
            targetShield.Value = Mathf.Max(targetShield.Value - damageAbsorbedByShield, 0);
                    
            // Apply the remaining damage to the target's health
            targetHealth.Value = Mathf.Clamp(targetHealth.Value - damageToTarget, 0, int.MaxValue);
        }
    }
}