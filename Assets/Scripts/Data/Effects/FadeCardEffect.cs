using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DefaultNamespace
{
    [CreateAssetMenu(menuName = "Gamejam/Effect/Fade Card Effect", fileName = "New Fade Card Effect")]
    public class FadeEffect : EffectData
    {
        public override IEnumerator Execute(RuntimeCard card, RuntimeCharacter player, RuntimeCharacter target, List<RuntimeCharacter> enemies)
        {
            // TODO: Fade the card (visuals + effect)
            
            card.cardState = CardState.FADED;
            
            player.properties.Get<int>(PropertyKey.CARDS_FADED_ON_CURRENT_TURN_COUNT).Value++;
            player.properties.Get<int>(PropertyKey.CARDS_FADED_ON_CURRENT_BATTLE_COUNT).Value++;

            // TODO: Execute passive/active skills that trigger on CardEvent.CARD_FADED
            
            throw new System.NotImplementedException();
        }

        public override string GetDescriptionText(RuntimeCard card, RuntimeCharacter player)
        {
            return "FADE";
        }
    }
}