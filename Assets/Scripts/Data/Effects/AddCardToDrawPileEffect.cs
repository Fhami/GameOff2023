using System;
using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

namespace DefaultNamespace
{
    [CreateAssetMenu(menuName = "Gamejam/Effect/Add Card To Draw Pile Effect", fileName = "New Add Card To Draw Pile Effect")]
    public class AddCardToDrawPileEffect : EffectData
    {
        public CardData cardToAdd;
        
        [Header("Times")]
        public ValueSource timesValueSource;
        
        [ShowIf("timesValueSource", ValueSource.CARD)]
        public int timesValue;
        
        [ShowIf("timesValueSource", ValueSource.CUSTOM)]
        public CustomValueSource customTimesValue;
        
        [ResizableTextArea]
        [ShowIf("timesValueSource", ValueSource.CUSTOM)]
        public string timesValueDescription;
        
        public override IEnumerator Execute(RuntimeCard card, RuntimeCharacter characterPlayingTheCard, RuntimeCharacter player, RuntimeCharacter cardTarget, List<RuntimeCharacter> enemies)
        {
            RuntimeCard runtimeCard = CardFactory.Create(cardToAdd);
            
            yield return BattleManager.current.CreateCardAndAddItToDrawPile(runtimeCard);
        }

        public override string GetDescriptionTextWithModifiers(RuntimeCard card, RuntimeCharacter characterPlayingTheCard, RuntimeCharacter player, RuntimeCharacter cardTarget, List<RuntimeCharacter> enemies)
        {
            switch (timesValueSource)
            {
                case ValueSource.NONE:
                {
                    int value = GetTimesValue(card, characterPlayingTheCard, player, cardTarget, enemies);
                    return $"Add {value.ToString()} {cardToAdd.name} into your draw pile.";
                }
                case ValueSource.CARD:
                {
                    int value = GetTimesValue(card, characterPlayingTheCard, player, cardTarget, enemies);
                    return $"Add {value.ToString()} {cardToAdd.name} into your draw pile.";break;
                }
                case ValueSource.CUSTOM:
                {
                    return $"Add number of {cardToAdd.name} to player draw pile {customTimesValue.GetDescription()}";
                }
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public override string GetDescriptionText()
        {
            switch (timesValueSource)
            {
                case ValueSource.NONE:
                {
                    return $"Add {GetTimesValue(null)} {cardToAdd.name} into your draw pile.";
                }
                case ValueSource.CARD:
                {
                    return $"Add {GetTimesValue(null)} {cardToAdd.name} into your draw pile.";break;
                }
                case ValueSource.CUSTOM:
                {
                    return $"Add number of {cardToAdd.name} to player draw pile {customTimesValue.GetDescription()}";
                }
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public override int GetEffectValue(RuntimeCard card, RuntimeCharacter characterPlayingTheCard, RuntimeCharacter player, RuntimeCharacter cardTarget, List<RuntimeCharacter> enemies)
        {
            throw new System.NotImplementedException();
        }

        public override string GetEffectValue(RuntimeCard card = null)
        {
            throw new System.NotImplementedException();
        }

        public override int GetTimesValue(RuntimeCard card, RuntimeCharacter characterPlayingTheCard, RuntimeCharacter player, RuntimeCharacter cardTarget, List<RuntimeCharacter> enemies)
        {
            int value = timesValueSource switch
            {
                ValueSource.NONE => 1,
                ValueSource.CARD => timesValue + card.properties.Get<int>(PropertyKey.TIMES).GetValueWithModifiers(card),
                ValueSource.CUSTOM => customTimesValue.GetValue(card, characterPlayingTheCard, player, cardTarget, enemies),
                _ => throw new ArgumentOutOfRangeException()
            };

            return value;
        }

        public override string GetTimesValue(RuntimeCard card = null)
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
                ValueSource.CARD => (timesValue + card.properties.Get<int>(PropertyKey.TIMES).GetValueWithModifiers(card)).ToString(),
                ValueSource.CUSTOM => "X",
                _ => throw new ArgumentOutOfRangeException()
            };
        }
    }
}