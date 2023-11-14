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
            RuntimeCharacter player,
            RuntimeCharacter cardTarget,
            List<RuntimeCharacter> enemies)
        {
            // TODO: Destroy the card (visuals + effect)

            card.properties.Get<CardState>(PropertyKey.CARD_STATE).Value = CardState.DESTROYED;
            
            characterPlayingTheCard.properties.Get<int>(PropertyKey.CARDS_DESTROYED_ON_CURRENT_TURN_COUNT).Value++;
            characterPlayingTheCard.properties.Get<int>(PropertyKey.CARDS_DESTROYED_ON_CURRENT_BATTLE_COUNT).Value++;
            
            yield return BattleManager.OnGameEvent(GameEvent.ON_CARD_DESTROYED, characterPlayingTheCard, player, enemies);
        }

        public override string GetDescriptionTextWithModifier(RuntimeCard card, RuntimeCharacter playerCharacter)
        {
            return "Destroy.";
        }

        public override string GetDescriptionText()
        {
            return "Destroy.";
        }

        protected override string GetDescriptionText(string value)
        {
            return "Destroy.";
        }
    }
}