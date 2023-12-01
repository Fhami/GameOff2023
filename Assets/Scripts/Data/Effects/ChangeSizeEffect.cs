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
        [Header("Operation")]
        public Operation operation;
        
        [Header("Target")]
        public EffectTarget effectTarget;
        
        [Header("Size")]
        public ValueSource sizeValueSource;
        
        [ShowIf("sizeValueSource", ValueSource.CARD)]
        public int sizeValue;

        [ShowIf("sizeValueSource", ValueSource.CUSTOM)]
        public CustomValueSource customSizeValue;

        [ResizableTextArea]
        [ShowIf("sizeValueSource", ValueSource.CUSTOM)]
        public string sizeValueDescription;
        
        
        public override IEnumerator Execute(RuntimeCard card, RuntimeCharacter characterPlayingTheCard, RuntimeCharacter player, RuntimeCharacter cardTarget, List<RuntimeCharacter> enemies)
        {
            List<RuntimeCharacter> targets = new();

            // Get the affected targets for the size change effect
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

            // Process the change size effect to every target
            int size = GetEffectValue(card, characterPlayingTheCard, player, cardTarget, enemies, out _);
            int times = GetTimesValue(card, characterPlayingTheCard, player, cardTarget, enemies);
            
            for (int i = 0; i < times; i++)
            {
                foreach (RuntimeCharacter target in targets)
                {
                    yield return ChangeSize(target, size, player, enemies);
                }
            }
        }

        public override string GetDescriptionTextWithModifiers(RuntimeCard card, RuntimeCharacter characterPlayingTheCard, RuntimeCharacter player, RuntimeCharacter cardTarget, List<RuntimeCharacter> enemies)
        {
            StringBuilder sb = new();

            switch (sizeValueSource)
            {
                case ValueSource.NONE:
                    break;
                case ValueSource.CARD:
                {
                    switch (effectTarget)
                    {
                        case EffectTarget.NONE:
                            throw new NotSupportedException();
                        case EffectTarget.PLAYER:
                            switch (operation)
                            {
                                case Operation.INCREASE:
                                {
                                    int value = GetEffectValue(card, characterPlayingTheCard, player, cardTarget, enemies, out ValueState valueState);
                                    sb.Append($"Player <color={Colors.COLOR_STATUS}>size</color> +<color={Colors.GetNumberColor(valueState)}>{value.ToString()}</color>");
                                    break;
                                }
                                case Operation.DECREASE:
                                {
                                    int value = GetEffectValue(card, characterPlayingTheCard, player, cardTarget, enemies, out ValueState valueState);
                                    sb.Append($"Player <color={Colors.COLOR_STATUS}>size</color> -<color={Colors.GetNumberColor(valueState)}>{value.ToString()}</color>");
                                    break;
                                }
                                case Operation.SET:
                                {
                                    int value = GetEffectValue(card, characterPlayingTheCard, player, cardTarget, enemies, out ValueState valueState);
                                    sb.Append($"Set player <color={Colors.COLOR_STATUS}>size</color> to <color={Colors.GetNumberColor(valueState)}>{value.ToString()}</color>");
                                    break;
                                }
                                default:
                                    throw new ArgumentOutOfRangeException();
                            }
                            break;
                        case EffectTarget.CARD_PLAYER:
                            switch (operation)
                            {
                                case Operation.INCREASE:
                                {
                                    int value = GetEffectValue(card, characterPlayingTheCard, player, cardTarget, enemies, out ValueState valueState);
                                    sb.Append($"<color={Colors.COLOR_STATUS}>Size</color> +<color={Colors.GetNumberColor(valueState)}>{value.ToString()}</color>");
                                    break;
                                }
                                case Operation.DECREASE:
                                {
                                    int value = GetEffectValue(card, characterPlayingTheCard, player, cardTarget, enemies, out ValueState valueState);
                                    sb.Append($"<color={Colors.COLOR_STATUS}>Size</color> -<color={Colors.GetNumberColor(valueState)}>{value.ToString()}</color>");
                                    break;
                                }
                                case Operation.SET:
                                {
                                    int value = GetEffectValue(card, characterPlayingTheCard, player, cardTarget, enemies, out ValueState valueState);
                                    sb.Append($"Set <color={Colors.COLOR_STATUS}>size</color> to <color={Colors.GetNumberColor(valueState)}>{value.ToString()}</color>");
                                    break;
                                }
                                default:
                                    throw new ArgumentOutOfRangeException();
                            }
                            break;
                        case EffectTarget.TARGET:
                            switch (operation)
                            {
                                case Operation.INCREASE:
                                {
                                    int value = GetEffectValue(card, characterPlayingTheCard, player, cardTarget, enemies, out ValueState valueState);
                                    sb.Append($"Target <color={Colors.COLOR_STATUS}>size</color> +<color={Colors.GetNumberColor(valueState)}>{value.ToString()}</color>");
                                    break;
                                }
                                case Operation.DECREASE:
                                {
                                    int value = GetEffectValue(card, characterPlayingTheCard, player, cardTarget, enemies, out ValueState valueState);
                                    sb.Append($"Target <color={Colors.COLOR_STATUS}>size</color> -<color={Colors.GetNumberColor(valueState)}>{value.ToString()}</color>");
                                    break;
                                }
                                case Operation.SET:
                                {
                                    int value = GetEffectValue(card, characterPlayingTheCard, player, cardTarget, enemies, out ValueState valueState);
                                    sb.Append($"Set target <color={Colors.COLOR_STATUS}>size</color> to <color={Colors.GetNumberColor(valueState)}>{value.ToString()}</color>");
                                    break;
                                }
                                default:
                                    throw new ArgumentOutOfRangeException();
                            }
                            break;
                        case EffectTarget.ALL_ENEMIES:
                            switch (operation)
                            {
                                case Operation.INCREASE:
                                {
                                    int value = GetEffectValue(card, characterPlayingTheCard, player, cardTarget, enemies, out ValueState valueState);
                                    sb.Append($"All enemies <color={Colors.COLOR_STATUS}>size</color> +<color={Colors.GetNumberColor(valueState)}>{value.ToString()}</color>");
                                    break;
                                }
                                case Operation.DECREASE:
                                {
                                    int value = GetEffectValue(card, characterPlayingTheCard, player, cardTarget, enemies, out ValueState valueState);
                                    sb.Append($"All enemies <color={Colors.COLOR_STATUS}>size</color> -<color={Colors.GetNumberColor(valueState)}>{value.ToString()}</color>");
                                    break;
                                }
                                case Operation.SET:
                                {
                                    int value = GetEffectValue(card, characterPlayingTheCard, player, cardTarget, enemies, out ValueState valueState);
                                    sb.Append($"Set all enemies <color={Colors.COLOR_STATUS}>size</color> to <color={Colors.GetNumberColor(valueState)}>{value.ToString()}</color>");
                                    break;
                                }
                                default:
                                    throw new ArgumentOutOfRangeException();
                            }
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                    break;
                }
                case ValueSource.CUSTOM:
                    sb.Append(sizeValueDescription);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            
            switch (timesValueSource)
            {
                case ValueSource.NONE:
                    break;
                case ValueSource.CARD:
                    sb.Append($" {GetTimesValue(card, characterPlayingTheCard, player, cardTarget, enemies).ToString()} times");
                    break;
                case ValueSource.CUSTOM:
                    sb.Append(" " + customTimesDescription);
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

            switch (sizeValueSource)
            {
                case ValueSource.NONE:
                    break;
                case ValueSource.CARD:
                {
                    switch (effectTarget)
                    {
                        case EffectTarget.NONE:
                            throw new NotSupportedException();
                        case EffectTarget.PLAYER:
                            switch (operation)
                            {
                                case Operation.INCREASE:
                                    sb.Append($"Player <color={Colors.COLOR_STATUS}>size</color> +{GetEffectValue()}");
                                    break;
                                case Operation.DECREASE:
                                    sb.Append($"Player <color={Colors.COLOR_STATUS}>size</color> -{GetEffectValue()}");
                                    break;
                                case Operation.SET:
                                    sb.Append($"Set player <color={Colors.COLOR_STATUS}>size</color> to {GetEffectValue()}");
                                    break;
                                default:
                                    throw new ArgumentOutOfRangeException();
                            }
                            break;
                        case EffectTarget.CARD_PLAYER:
                            switch (operation)
                            {
                                case Operation.INCREASE:
                                    sb.Append($"<color={Colors.COLOR_STATUS}>Size</color> +{GetEffectValue()}");
                                    break;
                                case Operation.DECREASE:
                                    sb.Append($"<color={Colors.COLOR_STATUS}>Size</color> -{GetEffectValue()}");
                                    break;
                                case Operation.SET:
                                    sb.Append($"Set <color={Colors.COLOR_STATUS}>size</color> to {GetEffectValue()}");
                                    break;
                                default:
                                    throw new ArgumentOutOfRangeException();
                            }
                            break;
                        case EffectTarget.TARGET:
                            switch (operation)
                            {
                                case Operation.INCREASE:
                                    sb.Append($"Target <color={Colors.COLOR_STATUS}>size</color> +{GetEffectValue()}");
                                    break;
                                case Operation.DECREASE:
                                    sb.Append($"Target <color={Colors.COLOR_STATUS}>size</color> -{GetEffectValue()}");
                                    break;
                                case Operation.SET:
                                    sb.Append($"Set target <color={Colors.COLOR_STATUS}>size</color> to {GetEffectValue()}");
                                    break;
                                default:
                                    throw new ArgumentOutOfRangeException();
                            }
                            break;
                        case EffectTarget.ALL_ENEMIES:
                            switch (operation)
                            {
                                case Operation.INCREASE:
                                    sb.Append($"All enemies <color={Colors.COLOR_STATUS}>size</color> +{GetEffectValue()}");
                                    break;
                                case Operation.DECREASE:
                                    sb.Append($"All enemies <color={Colors.COLOR_STATUS}>size</color> -{GetEffectValue()}");
                                    break;
                                case Operation.SET:
                                    sb.Append($"Set all enemies <color={Colors.COLOR_STATUS}>size</color> to {GetEffectValue()}");
                                    break;
                                default:
                                    throw new ArgumentOutOfRangeException();
                            }
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                    break;
                }
                case ValueSource.CUSTOM:
                    sb.Append(sizeValueDescription);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            
            switch (timesValueSource)
            {
                case ValueSource.NONE:
                    break;
                case ValueSource.CARD:
                    sb.Append($" {GetTimesValue()} times");
                    break;
                case ValueSource.CUSTOM:
                    sb.Append(" " + customTimesDescription);
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
            
            Property<int> stable = target.properties.Get<int>(PropertyKey.STABLE);
            Property<int> size = target.properties.Get<int>(PropertyKey.SIZE);
            Property<int> maxSize = target.properties.Get<int>(PropertyKey.MAX_SIZE);

            // Keep track of the previous size
            int previousSize = size.Value;

            int sizeChange = incomingSizeChange;
            
            // If the operation is INCREASE or DECREASE and not SET, then stable will reduce the size change
            if (operation != Operation.SET)
            {
                // Calculate the the size change value after stable absorption (i.e. reduce stable value from size change value)
                int amountAbsorbedByStable = Mathf.Min(incomingSizeChange, stable.Value);
                sizeChange = incomingSizeChange - amountAbsorbedByStable;
                
                // Reduce the absorbed size change value from the stable stack
                stable.Value = Mathf.Max(stable.Value - amountAbsorbedByStable, 0);
            }
            
            // Change the size based on the operation
            switch (operation)
            {
                case Operation.INCREASE:
                    size.Value = Mathf.Clamp(size.Value + sizeChange, target.characterData.minSize, maxSize.GetValueWithModifiers(target));
                    break;
                case Operation.DECREASE:
                    size.Value = Mathf.Clamp(size.Value - sizeChange, target.characterData.minSize, maxSize.GetValueWithModifiers(target));
                    break;
                case Operation.SET:
                    size.Value = Mathf.Clamp(sizeChange, target.characterData.minSize, maxSize.GetValueWithModifiers(target));
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

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
        
        /// <summary>
        /// Get the size value inside a battle. Calculates the final value with all the modifiers.
        /// </summary>
        public override int GetEffectValue(
            RuntimeCard card,
            RuntimeCharacter characterPlayingTheCard,
            RuntimeCharacter player,
            RuntimeCharacter cardTarget,
            List<RuntimeCharacter> enemies,
            out ValueState valueState)
        {
            int value = sizeValueSource switch
            {
                ValueSource.NONE => throw new NotSupportedException(),
                ValueSource.CARD => sizeValue,
                ValueSource.CUSTOM => customSizeValue.GetValue(card, characterPlayingTheCard, player, cardTarget, enemies),
                _ => throw new ArgumentOutOfRangeException()
            };
            
            int modifiers = card.properties.Get<int>(PropertyKey.SIZE).GetValueWithModifiers(card);

            int valueWithModifiers = value + modifiers;
            
            valueState = ValueState.NORMAL;
            if (valueWithModifiers > value)
            {
                valueState = ValueState.INCREASED;
            }
            else if (valueWithModifiers < value)
            {
                valueState = ValueState.DECREASED;
            }
            
            return valueWithModifiers;
        }

        /// <summary>
        /// Get size value outside the battle. If you have a reference to the card instance
        /// the method will also calculate the card upgrades into the final value.
        /// </summary>
        public override string GetEffectValue(RuntimeCard card = null)
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
        
    }
}