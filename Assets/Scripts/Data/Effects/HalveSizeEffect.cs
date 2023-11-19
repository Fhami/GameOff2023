using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DefaultNamespace
{
    [CreateAssetMenu(menuName = "Gamejam/Effect/Halve Size Effect", fileName = "New Halve Size Effect")]
    public class HalveSizeEffect : EffectData
    {
        [Header("Target")]
        public EffectTarget effectTarget;
        
        public override IEnumerator Execute(RuntimeCard card, RuntimeCharacter characterPlayingTheCard, RuntimeCharacter player, RuntimeCharacter cardTarget, List<RuntimeCharacter> enemies)
        {
            List<RuntimeCharacter> targets = new();

            // Get the affected targets
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
            
            foreach (RuntimeCharacter target in targets)
            {
                yield return HalveSize(target, player, enemies);
            }
        }

        public override string GetDescriptionTextWithModifiers(RuntimeCard card, RuntimeCharacter characterPlayingTheCard, RuntimeCharacter player, RuntimeCharacter cardTarget, List<RuntimeCharacter> enemies)
        {
            return GetDescriptionText();
        }

        public override string GetDescriptionText()
        {
            switch (effectTarget)
            {
                case EffectTarget.NONE:
                    throw new NotSupportedException();
                case EffectTarget.PLAYER:
                    return "Halve player size.";
                case EffectTarget.CARD_PLAYER:
                    return "Halve size.";
                case EffectTarget.TARGET:
                    return "Halve target size.";
                case EffectTarget.ALL_ENEMIES:
                    return "Halve size of all enemies.";
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private IEnumerator HalveSize(RuntimeCharacter target, RuntimeCharacter player, List<RuntimeCharacter> enemies)
        {
            // Get the target's form before we change it's size
            FormData previousForm = target.GetCurrentForm();
            
            Property<int> stable = target.properties.Get<int>(PropertyKey.STABLE);
            Property<int> size = target.properties.Get<int>(PropertyKey.SIZE);
            Property<int> maxSize = target.properties.Get<int>(PropertyKey.MAX_SIZE);

            // Keep track of the previous size
            int previousSize = size.Value;

            // Divide the target size by 2 (round up)
            int targetHalfSize = (int)Math.Ceiling(size.Value / 2.0);;
            int amountToReduce = Mathf.Max(0, previousSize - targetHalfSize);
            
            // Calculate the the size change value after stable absorption (i.e. reduce stable value from size change value)
            int amountAbsorbedByStable = Mathf.Min(amountToReduce, stable.Value);
            amountToReduce -= amountAbsorbedByStable;
                
            // Reduce the absorbed size change value from the stable stack
            stable.Value = Mathf.Max(stable.Value - amountAbsorbedByStable, 0);
            
            // Reduce the target size
            size.Value = Mathf.Clamp(size.Value - amountToReduce, 0, maxSize.GetValueWithModifiers(target));

            FormData currentForm = target.GetCurrentForm();

            if (previousForm != currentForm)
            {
                yield return BattleManager.current.ChangeForm(previousForm, currentForm, target, player, enemies);
            }
            
            if (previousSize != size.Value)
            {
                yield return BattleManager.current.ChangeSize(previousSize, size.Value, target, player, enemies);
            }
        }
    }
}