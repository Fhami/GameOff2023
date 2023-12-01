using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DefaultNamespace
{
    [CreateAssetMenu(menuName = "Gamejam/Effect/Flee Effect", fileName = "New Flee Effect")]
    public class FleeEffect : EffectData
    {
        public override IEnumerator Execute(RuntimeCard card, RuntimeCharacter characterPlayingTheCard, RuntimeCharacter player, RuntimeCharacter cardTarget, List<RuntimeCharacter> enemies)
        {
            yield return BattleManager.current.FleeFromBattle(characterPlayingTheCard);
        }

        public override string GetDescriptionTextWithModifiers(RuntimeCard card, RuntimeCharacter characterPlayingTheCard, RuntimeCharacter player, RuntimeCharacter cardTarget, List<RuntimeCharacter> enemies)
        {
            return GetDescriptionText();
        }

        public override string GetDescriptionText()
        {
            return "Flee.";
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
            return "0";
        }
    }
}