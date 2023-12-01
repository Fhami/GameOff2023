using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DefaultNamespace
{
    [CreateAssetMenu(menuName = "Gamejam/Effect/Fade Card Effect", fileName = "New Fade Card Effect")]
    public class FadeEffect : EffectData
    {
        public override IEnumerator Execute(RuntimeCard card, RuntimeCharacter characterPlayingTheCard, RuntimeCharacter player, RuntimeCharacter cardTarget, List<RuntimeCharacter> enemies)
        {
            yield return BattleManager.current.ExhaustCard(card, characterPlayingTheCard, player, cardTarget, enemies);
        }

        public override string GetDescriptionTextWithModifiers(RuntimeCard card, RuntimeCharacter characterPlayingTheCard, RuntimeCharacter player, RuntimeCharacter cardTarget, List<RuntimeCharacter> enemies)
        {
            return GetDescriptionText();
        }

        public override string GetDescriptionText()
        {
            return $"<color={Colors.COLOR_STATUS}>Fade</color>.";
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
            return 1;
        }

        public override string GetEffectValue(RuntimeCard card = null)
        {
            return "1";
        }
    }
}