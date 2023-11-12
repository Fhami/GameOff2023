using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DefaultNamespace
{
    [CreateAssetMenu(menuName = "Gamejam/Effect/Destroy Card Effect", fileName = "New Destroy Card Effect")]
    public class DestroyCardEffect : EffectData
    {
        public override IEnumerator Execute(
            RuntimeCard card,
            RuntimeCharacter characterPlayingTheCard,
            RuntimeCharacter playerCharacter,
            RuntimeCharacter targetCharacter,
            List<RuntimeCharacter> enemyCharacters)
        {
            // TODO: Destroy the card (visuals + effect)

            card.properties.Get<CardState>(PropertyKey.CARD_STATE).Value = CardState.DESTROYED;
            
            characterPlayingTheCard.properties.Get<int>(PropertyKey.CARDS_DESTROYED_ON_CURRENT_TURN_COUNT).Value++;
            characterPlayingTheCard.properties.Get<int>(PropertyKey.CARDS_DESTROYED_ON_CURRENT_BATTLE_COUNT).Value++;
            
            // TODO: Execute passive/active skills that trigger on CardEvent.CARD_DESTROYED
            
            yield break;
        }

        public override string GetDescriptionText(RuntimeCard card, RuntimeCharacter playerCharacter)
        {
            return "Destroy.";
        }
    }
}