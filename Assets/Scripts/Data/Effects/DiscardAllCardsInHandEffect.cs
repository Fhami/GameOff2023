using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DefaultNamespace
{
    [CreateAssetMenu(menuName = "Gamejam/Effect/Discard All Cards In Hand Effect", fileName = "New Discard All Cards In Hand Effect")]
    public class DiscardAllCardsInHandEffect : EffectData
    {
        public override IEnumerator Execute(RuntimeCard card, RuntimeCharacter characterPlayingTheCard, RuntimeCharacter player, RuntimeCharacter cardTarget, List<RuntimeCharacter> enemies)
        {
            yield return BattleManager.current.DiscardHand(card);
        }

        public override string GetDescriptionTextWithModifiers(RuntimeCard card, RuntimeCharacter characterPlayingTheCard, RuntimeCharacter player, RuntimeCharacter cardTarget, List<RuntimeCharacter> enemies)
        {
            return GetDescriptionText();
        }

        public override string GetDescriptionText()
        {
            return "Discard all cards in your hand.";
        }

        public override int GetEffectValue(RuntimeCard card, RuntimeCharacter characterPlayingTheCard, RuntimeCharacter player,
            RuntimeCharacter cardTarget, List<RuntimeCharacter> enemies)
        {
            return 1;
        }

        public override string GetEffectValue(RuntimeCard card = null)
        {
            return "1";
        }

        public override int GetTimesValue(RuntimeCard card, RuntimeCharacter characterPlayingTheCard, RuntimeCharacter player,
            RuntimeCharacter cardTarget, List<RuntimeCharacter> enemies)
        {
            return 1;
        }

        public override string GetTimesValue(RuntimeCard card = null)
        {
            return "1";
        }
    }
}