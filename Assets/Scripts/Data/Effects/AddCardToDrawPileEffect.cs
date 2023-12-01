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

        public override IEnumerator Execute(RuntimeCard card, RuntimeCharacter characterPlayingTheCard, RuntimeCharacter player, RuntimeCharacter cardTarget, List<RuntimeCharacter> enemies)
        {
            int times = GetTimesValue(card, characterPlayingTheCard, player, cardTarget, enemies);
            for (int i = 0; i < times; i++)
            {
                //Need new instance of runtime card for every card
                RuntimeCard runtimeCard = CardFactory.Create(cardToAdd);
                
                yield return BattleManager.current.CreateCardAndAddItToDiscardPile(runtimeCard);
            }
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

        public override int GetEffectValue(
            RuntimeCard card,
            RuntimeCharacter characterPlayingTheCard,
            RuntimeCharacter player,
            RuntimeCharacter cardTarget,
            List<RuntimeCharacter> enemies,
            out ValueState valueState)
        {
            valueState = ValueState.NORMAL;
            return 0;
        }

        public override string GetEffectValue(RuntimeCard card = null)
        {
            return "";
        }
        
    }
}