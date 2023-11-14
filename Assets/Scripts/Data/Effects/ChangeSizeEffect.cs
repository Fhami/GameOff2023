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
            int sizeChangeValueWithModifiers = GetCardSizeChangeValueWithModifiers(card);
            
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
                yield return ChangeSize(target, sizeChangeValueWithModifiers, player, enemies);
            }
        }

        public override string GetDescriptionTextWithModifier(RuntimeCard card, RuntimeCharacter playerCharacter)
        {
            int cardSizeWithModifiers = GetCardSizeChangeValueWithModifiers(card);

            return GetDescriptionText(cardSizeWithModifiers.ToString());
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
                case EffectTarget.PLAYER:
                {
                    switch (operation)
                    {
                        case Operation.INCREASE:
                            return $"Increase player size by {value.ToString()}.";
                        case Operation.DECREASE:
                            return $"Decrease player size by {value.ToString()}.";
                        case Operation.SET:
                            return $"Set player size to {value.ToString()}.";
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }
                case EffectTarget.CARD_PLAYER:
                {
                    switch (operation)
                    {
                        case Operation.INCREASE:
                            return $"Increase own size by {value.ToString()}.";
                        case Operation.DECREASE:
                            return $"Decrease own size by {value.ToString()}.";
                        case Operation.SET:
                            return $"Set own size to {value.ToString()}.";
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }
                case EffectTarget.TARGET:
                {
                    switch (operation)
                    {
                        case Operation.INCREASE:
                            return $"Increase target size by {value.ToString()}.";
                        case Operation.DECREASE:
                            return $"Decrease target size by {value.ToString()}.";
                        case Operation.SET:
                            return $"Set target size to {value.ToString()}.";
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }
                case EffectTarget.ALL_ENEMIES:
                {
                    switch (operation)
                    {
                        case Operation.INCREASE:
                            return $"Increase size of all enemies by {value.ToString()}.";
                        case Operation.DECREASE:
                            return $"Decrease size of all enemies by {value.ToString()}.";
                        case Operation.SET:
                            return $"Set size of all enemies to {value.ToString()}.";
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private IEnumerator ChangeSize(RuntimeCharacter target, int incomingSizeChange, RuntimeCharacter player, List<RuntimeCharacter> enemies)
        {
            // Get the target's form before we change it's size
            FormData previousForm = target.GetCurrentForm();
            
            Property<int> size = target.properties.Get<int>(PropertyKey.SIZE);
            Property<int> maxSize = target.properties.Get<int>(PropertyKey.MAX_SIZE);

            // Keep track of the previous size
            int previousSize = size.Value;

            // Change the size based on the operation
            switch (operation)
            {
                case Operation.INCREASE:
                    size.Value = Mathf.Clamp(size.Value - incomingSizeChange, 0, maxSize.GetValueWithModifiers(target));
                    break;
                case Operation.DECREASE:
                    size.Value = Mathf.Clamp(size.Value + incomingSizeChange, 0, maxSize.GetValueWithModifiers(target));
                    break;
                case Operation.SET:
                    size.Value = Mathf.Clamp(incomingSizeChange, 0, maxSize.GetValueWithModifiers(target));
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            FormData currentForm = target.GetCurrentForm();

            if (previousForm != currentForm)
            {
                yield return ChangeForm(previousForm, currentForm, target, player, enemies);
            }
            
            if (previousSize != size.Value)
            {
                yield return ChangeSize(previousSize, size.Value, target, player, enemies);
            }
        }
        
        private static IEnumerator ChangeForm(FormData previousForm, FormData currentForm, RuntimeCharacter character, RuntimeCharacter player, List<RuntimeCharacter> enemies)
        {
            character.properties.Get<int>(PropertyKey.FORM_CHANGED_COUNT_CURRENT_TURN).Value++;
            character.properties.Get<int>(PropertyKey.ENEMY_ATTACK_PATTERN_CARD_INDEX).Value = 0;

            // TODO: VFX, animation etc
            yield return BattleManager.OnGameEvent(GameEvent.ON_FORM_CHANGED, character, player, enemies);
        }
        
        private static IEnumerator ChangeSize(int previousSize, int currentSize, RuntimeCharacter character, RuntimeCharacter player, List<RuntimeCharacter> enemies)
        {
            // TODO: VFX, animation etc
            yield return BattleManager.OnGameEvent(GameEvent.ON_SIZE_CHANGED, character, player, enemies);
        }
        
        private int GetCardSizeChangeValueWithModifiers(RuntimeCard card)
        {
            int cardSizeChangeValueWithModifiers = card.properties.Get<int>(PropertyKey.SIZE).GetValueWithModifiers(card);
            return (int)value + cardSizeChangeValueWithModifiers;
        }
    }
}