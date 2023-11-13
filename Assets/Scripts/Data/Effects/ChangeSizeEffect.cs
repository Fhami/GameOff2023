using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DefaultNamespace
{
    [CreateAssetMenu(menuName = "Gamejam/Effect/Change Size Effect", fileName = "New Change Size Effect")]
    public class ChangeSizeEffect : EffectData
    {
        public Operation operation;
        public EffectTarget effectTarget;
        public uint value;

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

            // Process the size change to each target
            foreach (RuntimeCharacter target in targets)
            {
                // Get the target's form before we change it's size
                FormData previousForm = target.GetCurrentForm();
            
                // Change target size
                Property<int> size = target.properties.Get<int>(PropertyKey.SIZE);

                int previousSize = size.Value;
                
                int cardSizeWithModifiers = GetCardSizeWithModifiers(card);
                size.Value += cardSizeWithModifiers;

                int nextSize = size.Value;
                
                int maxSize = target.properties.Get<int>(PropertyKey.MAX_SIZE).GetValueWithModifiers(target);

                // If after size change the size is smaller or equal to 0
                if (size.Value <= 0)
                {
                    // Target takes 1 damage
                    Property<int> health = target.properties.Get<int>(PropertyKey.HEALTH);
                    health.Value -= 1;

                    // If target health dropped to <= 0 two things can happen
                    if (health.Value <= 0)
                    {
                        // TODO: DEATH/DUST! (both player and enemy can die here)
                    }
                    else
                    {
                        // Target still has 
                    }
                }
                else if (size.Value >= maxSize)
                {
                    // If 
                    if (target == player)
                    {
                        
                    }
                }
                
                // Get the target's form after we changed it's size
                FormData nextForm = target.GetCurrentForm();

                // If the form changed execute skills that trigger on ON_EXIT_FORM and ON_ENTER_FORM
                if (previousForm != nextForm)
                {
                    target.properties.Get<int>(PropertyKey.FORM_CHANGED_COUNT_CURRENT_TURN).Value++;
                    target.properties.Get<int>(PropertyKey.ENEMY_ATTACK_PATTERN_CARD_INDEX).Value = 0;
                    // TODO: Execute skills that trigger on ON_ENTER_FORM, ON_EXIT_FORM
                }
            
                // TODO: Execute active skill that triggers when we get certain size - to do this we need to check the skills of the character
            }
            
            yield break;
        }

        public override string GetDescriptionText(RuntimeCard card, RuntimeCharacter playerCharacter)
        {
            int cardSizeWithModifiers = GetCardSizeWithModifiers(card);
            
            switch (effectTarget)
            {
                case EffectTarget.NONE: throw new NotSupportedException();
                case EffectTarget.PLAYER:
                {
                    switch (operation)
                    {
                        case Operation.INCREASE:
                            return $"Increase player size by {cardSizeWithModifiers.ToString()}.";
                        case Operation.DECREASE:
                            return $"Decrease player size by {cardSizeWithModifiers.ToString()}.";
                        case Operation.SET:
                            return $"Set player size to {cardSizeWithModifiers.ToString()}.";
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }
                case EffectTarget.CARD_PLAYER:
                {
                    switch (operation)
                    {
                        case Operation.INCREASE:
                            return $"Increase own size by {cardSizeWithModifiers.ToString()}.";
                        case Operation.DECREASE:
                            return $"Decrease own size by {cardSizeWithModifiers.ToString()}.";
                        case Operation.SET:
                            return $"Set own size to {cardSizeWithModifiers.ToString()}.";
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }
                case EffectTarget.TARGET:
                {
                    switch (operation)
                    {
                        case Operation.INCREASE:
                            return $"Increase target size by {cardSizeWithModifiers.ToString()}.";
                        case Operation.DECREASE:
                            return $"Decrease target size by {cardSizeWithModifiers.ToString()}.";
                        case Operation.SET:
                            return $"Set target size to {cardSizeWithModifiers.ToString()}.";
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }
                case EffectTarget.ALL_ENEMIES:
                {
                    switch (operation)
                    {
                        case Operation.INCREASE:
                            return $"Increase size of all enemies by {cardSizeWithModifiers.ToString()}.";
                        case Operation.DECREASE:
                            return $"Decrease size of all enemies by {cardSizeWithModifiers.ToString()}.";
                        case Operation.SET:
                            return $"Set size of all enemies to {cardSizeWithModifiers.ToString()}.";
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
        
        private int GetCardSizeWithModifiers(RuntimeCard card)
        {
            int sizeWithModifiers = card.properties.Get<int>(PropertyKey.SIZE).GetValueWithModifiers(card);
            return (int)value + sizeWithModifiers;
        }
    }
}