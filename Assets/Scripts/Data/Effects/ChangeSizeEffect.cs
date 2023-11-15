using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using NaughtyAttributes;
using UnityEngine;

namespace DefaultNamespace
{
    [CreateAssetMenu(menuName = "Gamejam/Effect/Change Size Effect", fileName = "New Change Size Effect")]
    public class ChangeSizeEffect : EffectData
    {
        [Header("The size change operation")]
        public Operation operation;
        
        [Header("The target(s) the effect gets applied to")]
        public EffectTarget effectTarget;
        
        [Header("The source of the 'size' value")]
        public ValueSource sizeValueSource;
        
        [ShowIf("sizeValueSource", ValueSource.CARD)]
        public int sizeValue;
        
        [ShowIf("sizeValueSource", ValueSource.CUSTOM)]
        public DamageValueSource customSizeValue;

        [Header("The source of the 'times' value")]
        public ValueSource timesValueSource;
        
        [ShowIf("timesValueSource", ValueSource.CARD)]
        public int timesValue;
        
        [ShowIf("timesValueSource", ValueSource.CUSTOM)]
        public DamageValueSource customTimesValue;
        
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

            int size = GetSizeValue(card, characterPlayingTheCard, player, cardTarget, enemies);
            int times = GetTimesValue(card, characterPlayingTheCard, player, cardTarget, enemies);
            for (int i = 0; i < times; i++)
            {
                // Process the effect to every target
                foreach (RuntimeCharacter target in targets)
                {
                    yield return ChangeSize(target, size, player, enemies);
                }
            }
        }

        public override string GetDescriptionTextWithModifiers(RuntimeCard card,
            RuntimeCharacter characterPlayingTheCard,
            RuntimeCharacter player,
            RuntimeCharacter cardTarget,
            List<RuntimeCharacter> enemies)
        {
            StringBuilder sb = new();
            
            // Step 1) Build "Size -X" / "Size +X to all enemies" string
            switch (effectTarget)
            {
                case EffectTarget.NONE:
                    throw new NotSupportedException();
                case EffectTarget.PLAYER:
                    switch (operation)
                    {
                        case Operation.INCREASE:
                            sb.Append($"+{GetSizeValue(card, characterPlayingTheCard, player, cardTarget, enemies).ToString()} size to player");
                            break;
                        case Operation.DECREASE:
                            sb.Append($"-{GetSizeValue(card, characterPlayingTheCard, player, cardTarget, enemies).ToString()} size to player");
                            break;
                        case Operation.SET:
                            sb.Append($"Set player size to {GetSizeValue(card, characterPlayingTheCard, player, cardTarget, enemies).ToString()}");
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                    break;
                case EffectTarget.CARD_PLAYER:
                    switch (operation)
                    {
                        case Operation.INCREASE:
                            sb.Append($"+{GetSizeValue(card, characterPlayingTheCard, player, cardTarget, enemies).ToString()} size");
                            break;
                        case Operation.DECREASE:
                            sb.Append($"-{GetSizeValue(card, characterPlayingTheCard, player, cardTarget, enemies).ToString()} size");
                            break;
                        case Operation.SET:
                            sb.Append($"Set size to {GetSizeValue(card, characterPlayingTheCard, player, cardTarget, enemies).ToString()}");
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                    break;
                case EffectTarget.TARGET:
                    switch (operation)
                    {
                        case Operation.INCREASE:
                            sb.Append($"+{GetSizeValue(card, characterPlayingTheCard, player, cardTarget, enemies).ToString()} size to target");
                            break;
                        case Operation.DECREASE:
                            sb.Append($"-{GetSizeValue(card, characterPlayingTheCard, player, cardTarget, enemies).ToString()} size to target");
                            break;
                        case Operation.SET:
                            sb.Append($"Set target size to {GetSizeValue(card, characterPlayingTheCard, player, cardTarget, enemies).ToString()}");
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                    break;
                case EffectTarget.ALL_ENEMIES:
                    switch (operation)
                    {
                        case Operation.INCREASE:
                            sb.Append($"+{GetSizeValue(card, characterPlayingTheCard, player, cardTarget, enemies).ToString()} size to all enemies");
                            break;
                        case Operation.DECREASE:
                            sb.Append($"-{GetSizeValue(card, characterPlayingTheCard, player, cardTarget, enemies).ToString()} size to all enemies");
                            break;
                        case Operation.SET:
                            sb.Append($"Set size of all enemies to {GetSizeValue(card, characterPlayingTheCard, player, cardTarget, enemies).ToString()}");
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            
            // Step 2) Build " X times." / " number of times you've discarded cards this turn" string
            switch (timesValueSource)
            {
                case ValueSource.NONE:
                    break;
                case ValueSource.CARD:
                    sb.Append($" {GetTimesValue(card, characterPlayingTheCard, player, cardTarget, enemies).ToString()} times.");
                    break;
                case ValueSource.CUSTOM:
                    sb.Append(customTimesValue.GetDescription());
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            sb.Append(".");
            
            return sb.ToString();
        }

        public override string GetDescriptionText()
        {
                        StringBuilder sb = new();
            
            // Step 1) Build "Size -X" / "Size +X to all enemies" string
            switch (effectTarget)
            {
                case EffectTarget.NONE:
                    throw new NotSupportedException();
                case EffectTarget.PLAYER:
                    switch (operation)
                    {
                        case Operation.INCREASE:
                            sb.Append($"+{GetSizeValue()} size to player");
                            break;
                        case Operation.DECREASE:
                            sb.Append($"-{GetSizeValue()} size to player");
                            break;
                        case Operation.SET:
                            sb.Append($"Set player size to {GetSizeValue()}");
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                    break;
                case EffectTarget.CARD_PLAYER:
                    switch (operation)
                    {
                        case Operation.INCREASE:
                            sb.Append($"+{GetSizeValue()} size");
                            break;
                        case Operation.DECREASE:
                            sb.Append($"-{GetSizeValue()} size");
                            break;
                        case Operation.SET:
                            sb.Append($"Set size to {GetSizeValue()}");
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                    break;
                case EffectTarget.TARGET:
                    switch (operation)
                    {
                        case Operation.INCREASE:
                            sb.Append($"+{GetSizeValue()} size to target");
                            break;
                        case Operation.DECREASE:
                            sb.Append($"-{GetSizeValue()} size to target");
                            break;
                        case Operation.SET:
                            sb.Append($"Set target size to {GetSizeValue()}");
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                    break;
                case EffectTarget.ALL_ENEMIES:
                    switch (operation)
                    {
                        case Operation.INCREASE:
                            sb.Append($"+{GetSizeValue()} size to all enemies");
                            break;
                        case Operation.DECREASE:
                            sb.Append($"-{GetSizeValue()} size to all enemies");
                            break;
                        case Operation.SET:
                            sb.Append($"Set size of all enemies to {GetSizeValue()}");
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            
            // Step 2) Build " X times." / " number of times you've discarded cards this turn" string
            switch (timesValueSource)
            {
                case ValueSource.NONE:
                    break;
                case ValueSource.CARD:
                    sb.Append($" {GetTimesValue()} times.");
                    break;
                case ValueSource.CUSTOM:
                    sb.Append(customTimesValue.GetDescription());
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            sb.Append(".");
            
            return sb.ToString();
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
        
        private int GetSizeValue(RuntimeCard card,
            RuntimeCharacter characterPlayingTheCard,
            RuntimeCharacter player,
            RuntimeCharacter cardTarget,
            List<RuntimeCharacter> enemies)
        {
            int value = sizeValueSource switch
            {
                ValueSource.NONE => throw new NotSupportedException(),
                ValueSource.CARD => sizeValue,
                ValueSource.CUSTOM => customSizeValue.GetValue(card, characterPlayingTheCard, player, cardTarget, enemies),
                _ => throw new ArgumentOutOfRangeException()
            };
            
            int cardSizeWithModifiers = card.properties.Get<int>(PropertyKey.SIZE).GetValueWithModifiers(card);
            
            return value + cardSizeWithModifiers;
        }
        
        private int GetTimesValue(RuntimeCard card,
            RuntimeCharacter characterPlayingTheCard,
            RuntimeCharacter player,
            RuntimeCharacter cardTarget,
            List<RuntimeCharacter> enemies)
        {
            int value = timesValueSource switch
            {
                ValueSource.NONE => 1,
                ValueSource.CARD => timesValue,
                ValueSource.CUSTOM => customTimesValue.GetValue(card, characterPlayingTheCard, player, cardTarget, enemies),
                _ => throw new ArgumentOutOfRangeException()
            };

            return value;
        }
        
        /// <summary>
        /// Get size value outside the battle. If card is null we don't have card upgrades calculated in the value.
        /// </summary>
        private string GetSizeValue(RuntimeCard card = null)
        {
            if (card == null)
            {
                return sizeValueSource switch
                {
                    ValueSource.NONE => throw new NotSupportedException(),
                    ValueSource.CARD => sizeValue.ToString(),
                    ValueSource.CUSTOM => "X",
                    _ => throw new ArgumentOutOfRangeException()
                };
            }
          
            return sizeValueSource switch
            {
                ValueSource.NONE => throw new NotSupportedException(),
                ValueSource.CARD => (sizeValue + card.properties.Get<int>(PropertyKey.SIZE).GetValueWithModifiers(card)).ToString(),
                ValueSource.CUSTOM => "X",
                _ => throw new ArgumentOutOfRangeException()
            };
        }
        
        /// <summary>
        /// Get times value outside the battle. If card is null we don't have card upgrades calculated in the value.
        /// </summary>
        private string GetTimesValue(RuntimeCard card = null)
        {
            if (card == null)
            {
                return timesValueSource switch
                {
                    ValueSource.NONE => 1.ToString(),
                    ValueSource.CARD => timesValue.ToString(),
                    ValueSource.CUSTOM => "X",
                    _ => throw new ArgumentOutOfRangeException()
                };
            }
          
            return timesValueSource switch
            {
                ValueSource.NONE => 1.ToString(),
                ValueSource.CARD => (timesValue + card.properties.Get<int>(PropertyKey.ATTACK).GetValueWithModifiers(card)).ToString(),
                ValueSource.CUSTOM => "X",
                _ => throw new ArgumentOutOfRangeException()
            };
        }
    }
}