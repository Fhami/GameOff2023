using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DefaultNamespace
{
    [CreateAssetMenu(menuName = "Gamejam/Effect/Change Power Effect", fileName = "New Change Power Effect")]
    public class ChangePowerEffect : EffectData
    {
        public Operation operation;
        public EffectTarget effectTarget;
        public uint value;

        public override IEnumerator Execute(
            RuntimeCard card,
            RuntimeCharacter characterPlayingTheCard,
            RuntimeCharacter playerCharacter,
            RuntimeCharacter targetCharacter,
            List<RuntimeCharacter> enemyCharacters)
        {
            // TODO: VFX

            // Get affected targets based on the effect target
            List<RuntimeCharacter> targets = new();

            switch (effectTarget)
            {
                case EffectTarget.NONE:
                    throw new NotSupportedException();
                case EffectTarget.PLAYER:
                    targets.Add(playerCharacter);
                    break;
                case EffectTarget.CARD_PLAYER:
                    targets.Add(characterPlayingTheCard);
                    break;
                case EffectTarget.TARGET:
                    targets.Add(targetCharacter);
                    break;
                case EffectTarget.ALL_ENEMIES:
                    targets.AddRange(enemyCharacters);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            // Apply the effect to each target.
            foreach (RuntimeCharacter target in targets)
            {
                // Get the target's form before we change it's power
                target.TryGetCurrentForm(out FormData previousForm);
            
                // Change target's power
                Property<int> targetPower = target.properties.Get<int>(PropertyKey.POWER);
                int powerWithModifiers = GetCardPowerWithModifiers(card);
                targetPower.Value += powerWithModifiers;
            
                // Get the target's form after we changed it's power
                target.TryGetCurrentForm(out FormData nextForm);

                // If the form changed execute skills that trigger on ON_EXIT_FORM and ON_ENTER_FORM
                if (previousForm != nextForm)
                {
                    // TODO: Execute skills that trigger on ON_ENTER_FORM, ON_EXIT_FORM
                }
            
                // TODO: Execute active skill that triggers when we get certain power - to do this we need to check the skills of the character
            }
            
            yield break;
        }

        public override string GetDescriptionText(RuntimeCard card, RuntimeCharacter playerCharacter)
        {
            int cardPowerWithModifiers = GetCardPowerWithModifiers(card);
            
            switch (effectTarget)
            {
                case EffectTarget.NONE: throw new NotSupportedException();
                case EffectTarget.PLAYER: throw new NotSupportedException("Use CARD_PLAYER instead! (I don't think we need this?)");
                case EffectTarget.CARD_PLAYER:
                {
                    switch (operation)
                    {
                        case Operation.INCREASE:
                            return $"Increase own power by {cardPowerWithModifiers.ToString()}.";
                        case Operation.DECREASE:
                            return $"Decrease own power by {cardPowerWithModifiers.ToString()}.";
                        case Operation.SET:
                            return $"Set own power to {cardPowerWithModifiers.ToString()}.";
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }
                case EffectTarget.TARGET:
                {
                    switch (operation)
                    {
                        case Operation.INCREASE:
                            return $"Increase target power by {cardPowerWithModifiers.ToString()}.";
                        case Operation.DECREASE:
                            return $"Decrease target power by {cardPowerWithModifiers.ToString()}.";
                        case Operation.SET:
                            return $"Set target power to {cardPowerWithModifiers.ToString()}.";
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }
                case EffectTarget.ALL_ENEMIES:
                {
                    switch (operation)
                    {
                        case Operation.INCREASE:
                            return $"Increase power of all enemies by {cardPowerWithModifiers.ToString()}.";
                        case Operation.DECREASE:
                            return $"Decrease power of all enemies by {cardPowerWithModifiers.ToString()}.";
                        case Operation.SET:
                            return $"Set power of all enemies to {cardPowerWithModifiers.ToString()}.";
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
        
        private int GetCardPowerWithModifiers(RuntimeCard card)
        {
            int cardPowerWithModifiers = card.properties.Get<int>(PropertyKey.POWER).GetValueWithModifiers(card);
            return (int)value + cardPowerWithModifiers;
        }
    }
}