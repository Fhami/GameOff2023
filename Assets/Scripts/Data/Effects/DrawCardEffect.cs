using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
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
            
            int times = GetTimesValue(card, characterPlayingTheCard, player, cardTarget, enemies);
            var count = times + this.count;

            for (int i = 0; i < count; i++)
            {
                yield return BattleManager.current.DrawCard(card, characterPlayingTheCard, player, cardTarget, enemies);
            }
        }

        public override string GetDescriptionTextWithModifiers(RuntimeCard card, RuntimeCharacter characterPlayingTheCard, RuntimeCharacter player, RuntimeCharacter cardTarget, List<RuntimeCharacter> enemies)
        {
            StringBuilder sb = new();
            
            sb.Append($"Draw {count.ToString()} cards.");
            
            switch (timesValueSource)
            {
                case ValueSource.NONE:
                    break;
                case ValueSource.CARD:
                    sb.Append($" {GetTimesValue(card, characterPlayingTheCard, player, cardTarget, enemies).ToString()} times");
                    break;
                case ValueSource.CUSTOM:
                    sb.Append(" " + customTimesDescription);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            return sb.ToString();
        }

        public override string GetDescriptionText()
        {
            StringBuilder sb = new();

            sb.Append($"Draw {count.ToString()} cards.");
            
            switch (timesValueSource)
            {
                case ValueSource.NONE:
                    break;
                case ValueSource.CARD:
                    sb.Append($" {GetTimesValue()} times");
                    break;
                case ValueSource.CUSTOM:
                    sb.Append(" " + customTimesDescription);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            return sb.ToString();
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
    }
}