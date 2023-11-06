using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DefaultNamespace
{
    [CreateAssetMenu(menuName = "Gamejam/Effect/Destroy Card Effect", fileName = "New Destroy Card Effect")]
    public class DestroyCardEffect : EffectData
    {
        public override IEnumerator Execute(RuntimeCard card, RuntimeCharacter player, RuntimeCharacter target, List<RuntimeCharacter> enemies)
        {
            // TODO: Destroy the card (visuals + effect)

            card.cardState = CardState.DESTROYED;
            
            player.properties.Get<int>(PropertyKey.CARDS_DESTROYED_ON_CURRENT_TURN_COUNT).Value++;
            player.properties.Get<int>(PropertyKey.CARDS_DESTROYED_ON_CURRENT_BATTLE_COUNT).Value++;
            
            // TODO: Execute passive/active skills that trigger on CardEvent.CARD_DESTROYED
            
            throw new System.NotImplementedException();
        }

        public override string GetDescriptionText(RuntimeCard card, RuntimeCharacter player)
        {
            return "DESTROY";
        }
    }
}