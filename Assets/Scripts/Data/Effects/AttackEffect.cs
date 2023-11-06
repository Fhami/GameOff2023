using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace DefaultNamespace
{
    [CreateAssetMenu(menuName = "Gamejam/Effect/Attack Effect", fileName = "New Attack Effect")]
    public class AttackEffect : EffectData
    {
        public int damage;
        public bool aoe;
        
        public override IEnumerator Execute(RuntimeCard card, RuntimeCharacter player, RuntimeCharacter target, List<RuntimeCharacter> enemies)
        {
            // TODO: Deal damage (effect)

            // Get the damage with modifiers (card damage + modifiers from player)
            int incomingDamage = GetDamageWithModifiers(card, player);
            
            if (aoe) // Handle damage to multiple targets
            {
                foreach (RuntimeCharacter enemy in enemies)
                {
                    DealDamage(enemy, incomingDamage);
                }
            }
            else // Handle damage to single target 
            {
                DealDamage(target, incomingDamage);
            }
            
            // Clear power-up stack
            player.properties.Get<int>(PropertyKey.POWER_UP).Value = 0;
            
            throw new System.NotImplementedException();
        }

        public override string GetDescriptionText(RuntimeCard card, RuntimeCharacter player)
        {
            int calculatedDamage = GetDamageWithModifiers(card, player);
            // TODO: You can use rich text here to change the ATK value color if the ATK is modifier (you can just compare calculatedDamage with the card damage)
            return $"ATK {calculatedDamage}";
        }

        private int GetDamageWithModifiers(RuntimeCard card, RuntimeCharacter player)
        {
            // Card damage formula: damage + player attack + player power-up + card attack
            int playerAttack = player.properties.Get<int>(PropertyKey.ATTACK).GetValueWithModifiers(player);
            int playerPowerUp = player.properties.Get<int>(PropertyKey.POWER_UP).GetValueWithModifiers(player);
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