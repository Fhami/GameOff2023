using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DefaultNamespace
{
    [CreateAssetMenu(menuName = "Gamejam/Effect/Draw Card Effect", fileName = "New Draw Card Effect")]
    public class DrawCardEffect : EffectData
    {
        public int count;

        public override IEnumerator Execute(RuntimeCard card, RuntimeCharacter characterPlayingTheCard, RuntimeCharacter player, RuntimeCharacter cardTarget, List<RuntimeCharacter> enemies)
        {
            if (characterPlayingTheCard.properties.Get<bool>(PropertyKey.CANNOT_DRAW_ADDITIONAL_CARDS_CURRENT_TURN).Value)
            {
                // TODO: VFX / visuals what happens when can't draw?
                yield break;
            }

            yield return BattleManager.current.DrawCard(card, characterPlayingTheCard, player, cardTarget, enemies);
        }

        public override string GetDescriptionTextWithModifiers(RuntimeCard card, RuntimeCharacter characterPlayingTheCard, RuntimeCharacter player, RuntimeCharacter cardTarget, List<RuntimeCharacter> enemies)
        {
            return GetDescriptionText();
        }

        public override string GetDescriptionText()
        {
            return $"Draw {count.ToString()} cards.";
        }

        public override int GetEffectValue(RuntimeCard card, RuntimeCharacter characterPlayingTheCard, RuntimeCharacter player,
            RuntimeCharacter cardTarget, List<RuntimeCharacter> enemies)
        {
            return count;
        }

        public override string GetEffectValue(RuntimeCard card = null)
        {
            return count.ToString();
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