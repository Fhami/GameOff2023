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
            // TODO: Fade the card (visuals + effect) / put card to FADE pile
            
            /*
             * NOTE: Copied this description from Slay the Spire EXHAUST card. FADE is same as EXHAUST, right?
             * Exhausting a card puts it in your exhaust pile. Cards in this pile are effectively removed from combat and cannot be used again.
             * If you Exhaust a card that was in your deck, it will be returned to your deck at the end of combat.
             */
            
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