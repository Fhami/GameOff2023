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
                        yield return Attack(enemyCharacter, attackValueWithModifiers, playerCharacter, enemyCharacters);
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
                    yield return Attack(targetCharacter, attackValueWithModifiers, playerCharacter, enemyCharacters);
                }
            }
            else
            {
                throw new NotImplementedException("For now attack effect only supports TARGET and ALL_ENEMIES.");
            }
            
            // Clear character's strength stack after the attack
            characterPlayingTheCard.properties.Get<int>(PropertyKey.STRENGTH).Value = 0;
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

        private IEnumerator Attack(
            RuntimeCharacter target,
            int incomingAttack,
            RuntimeCharacter playerCharacter,
            List<RuntimeCharacter> enemyCharacters)
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

            // If the target's size dropped to 0 they'll take 1 damage to their health
            if (size.Value <= 0)
            {
                Property<int> health = target.properties.Get<int>(PropertyKey.HEALTH);
                Property<int> maxHealth = target.properties.Get<int>(PropertyKey.MAX_HEALTH);
                health.Value = Mathf.Clamp(health.Value - 1, 0, maxHealth.GetValueWithModifiers(target));
                
                // If the target's health dropped to 0 they will DIE, otherwise they change back to their starting size
                if (health.Value == 0)
                {
                    yield return Kill(target);
                    // TODO: Kill the target (it can be player or enemy)
                }
                else
                {
                    // TODO: Change back to starting size
                }
            }
            else // If the target size is still bigger than 0
            {
                // If the target has not been staggered yet and the new size matches the character's stagger size
                Property<bool> hasBeenStaggeredOnce = target.properties.Get<bool>(PropertyKey.HAS_BEEN_STAGGERED_ONCE);
                if (!hasBeenStaggeredOnce.Value && size.Value == target.characterData.staggerSize)
                {
                    hasBeenStaggeredOnce.Value = true;
                    yield return Stagger(target, playerCharacter, enemyCharacters);
                }
                // TODO: skills
            }
            
            // TODO: form change effects
            FormData formAfterAttack = target.GetCurrentForm();

            if (formBeforeAttack != formAfterAttack)
            {
                target.properties.Get<int>(PropertyKey.FORM_CHANGED_COUNT_CURRENT_TURN).Value++;
                target.properties.Get<int>(PropertyKey.ENEMY_ATTACK_PATTERN_CARD_INDEX).Value = 0;
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
        
        private IEnumerator Kill(RuntimeCharacter character)
        {
            // TODO: Kill VFX (+ apply possible on_death effects?)
            yield break;
        }
        
        private IEnumerator Stagger(
            RuntimeCharacter staggeredCharacter,
            RuntimeCharacter playerCharacter,
            List<RuntimeCharacter> enemyCharacters)
        {
            // TODO: Stagger VFX/animation?
            
            RuntimeCard staggerCard = CardFactory.Create(staggeredCharacter.characterData.staggerCard.name);

            foreach (EffectData staggerEffect in staggerCard.cardData.effects)
            {
                yield return staggerEffect.Execute(staggerCard, staggeredCharacter, playerCharacter, playerCharacter, enemyCharacters);
            }
        }
    }
}