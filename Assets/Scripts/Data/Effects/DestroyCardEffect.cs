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
            yield return BattleManager.current.DestroyCard(card, characterPlayingTheCard, player, cardTarget, enemies);
        }

        public override string GetDescriptionTextWithModifiers(RuntimeCard card,
            RuntimeCharacter characterPlayingTheCard,
            RuntimeCharacter player,
            RuntimeCharacter cardTarget,
            List<RuntimeCharacter> enemies)
        {
            return GetDescriptionText();
        }

        public override string GetDescriptionText()
        {
            return "Destroy.";
        }
    }
}