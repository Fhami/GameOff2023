using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DefaultNamespace
{
    [CreateAssetMenu(menuName = "Gamejam/Effect/Change Size Effect", fileName = "New Change Size Effect")]
    public class ChangeSizeEffect : EffectData
    {
        public uint value;
        public Operation operation;
        
        public override IEnumerator Execute(RuntimeCard card, RuntimeCharacter player, RuntimeCharacter target, List<RuntimeCharacter> enemies)
        {
            // TODO: visuals, vfx etc.

            // Get the target's form before we change it's size
            player.TryGetCurrentForm(out FormData formBeforeSizeChange);
            
            // Change the target's size
            Property<int> targetSize = target.properties.Get<int>(PropertyKey.SIZE);
            int size = GetSizeWithModifiers(card);
            targetSize.Value += size;
            
            // Get the target's form after we changed it's size
            player.TryGetCurrentForm(out FormData formAfterSizeChange);

            // If our size changed execute skills that trigger on ON_EXIT_FORM and ON_ENTER_FORM
            if (formBeforeSizeChange != formAfterSizeChange)
            {
                // TODO: Execute skills that trigger on ON_ENTER_FORM, ON_EXIT_FORM
            }
            
            // TODO: Execute active skill that triggers when we enter certain size. To do this we need to check the active skills of the character
            
            throw new NotImplementedException();
        }

        public override string GetDescriptionText(RuntimeCard card, RuntimeCharacter player)
        {
            if (operation == Operation.INCREASE)
            {
                return $"SIZE +{GetSizeWithModifiers(card)}";
            }
            else if (operation == Operation.DECREASE)
            {
                return $"SIZE -{GetSizeWithModifiers(card)}";
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        private int GetSizeWithModifiers(RuntimeCard card)
        {
            int cardSize = card.properties.Get<int>(PropertyKey.SIZE).GetValueWithModifiers(card);
            return (int)value + cardSize;
        }
    }
}