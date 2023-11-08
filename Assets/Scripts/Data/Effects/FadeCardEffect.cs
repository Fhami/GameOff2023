using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DefaultNamespace
{
    [CreateAssetMenu(menuName = "Gamejam/Effect/Fade Card Effect", fileName = "New Fade Card Effect")]
    public class FadeEffect : EffectData
    {
        public override IEnumerator Execute(
            RuntimeCard card,
            RuntimeCharacter characterPlayingTheCard,
            RuntimeCharacter playerCharacter,
            RuntimeCharacter targetCharacter,
            List<RuntimeCharacter> enemyCharacters)
        {
            // TODO: Fade the card (visuals + effect)
            
            card.cardState = CardState.FADED;
            
            characterPlayingTheCard.properties.Get<int>(PropertyKey.CARDS_FADED_ON_CURRENT_TURN_COUNT).Value++;
            characterPlayingTheCard.properties.Get<int>(PropertyKey.CARDS_FADED_ON_CURRENT_BATTLE_COUNT).Value++;

            // TODO: Execute passive/active skills that trigger on CardEvent.CARD_FADED
            
            yield break;
        }

        public override string GetDescriptionText(RuntimeCard card, RuntimeCharacter playerCharacter)
        {
            return "Fade.";
        }
    }
}