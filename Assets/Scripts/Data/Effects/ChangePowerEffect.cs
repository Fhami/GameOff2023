using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DefaultNamespace
{
    [CreateAssetMenu(menuName = "Gamejam/Effect/Change Power Effect", fileName = "New Change Power Effect")]
    public class ChangePowerEffect : EffectData
    {
        public uint value;
        public Operation operation;
        
        public override IEnumerator Execute(RuntimeCard card, RuntimeCharacter player, RuntimeCharacter target, List<RuntimeCharacter> enemies)
        {
            // TODO: visuals, vfx etc.

            // Get the target's form before we change it's power
            player.TryGetCurrentForm(out FormData formBeforePowerChange);
            
            // Change target's power
            Property<int> targetPower = target.properties.Get<int>(PropertyKey.POWER);
            int powerWithModifiers = GetPowerWithModifiers(card);
            targetPower.Value += powerWithModifiers;
            
            // Get the target's form after we changed it's power
            player.TryGetCurrentForm(out FormData formAfterPowerChange);

            // If the form changed execute skills that trigger on ON_EXIT_FORM and ON_ENTER_FORM
            if (formBeforePowerChange != formAfterPowerChange)
            {
                // TODO: Execute skills that trigger on ON_ENTER_FORM, ON_EXIT_FORM
            }
            
            // TODO: Execute active skill that triggers when we get certain power - to do this we need to check the skills of the character
            
            throw new NotImplementedException();
        }

        public override string GetDescriptionText(RuntimeCard card, RuntimeCharacter player)
        {
            if (operation == Operation.INCREASE)
            {
                return $"POWER +{GetPowerWithModifiers(card)}";
            }
            else if (operation == Operation.DECREASE)
            {
                return $"POWER -{GetPowerWithModifiers(card)}";
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        private int GetPowerWithModifiers(RuntimeCard card)
        {
            int cardPower = card.properties.Get<int>(PropertyKey.POWER).GetValueWithModifiers(card);
            return (int)value + cardPower;
        }
    }
}