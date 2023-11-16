using System.Collections.Generic;
using UnityEngine;

namespace DefaultNamespace
{
    [CreateAssetMenu(menuName = "Gamejam/Custom Value Source/Number Of Cards In Draw Pile", fileName = "Number Of Cards In Draw Pile")]
    public class CustomValueSource_NumberOfCardsInDrawPile : CustomValueSource
    {
        public override int GetValue(RuntimeCard card, RuntimeCharacter characterPlayingTheCard, RuntimeCharacter player, RuntimeCharacter cardTarget, List<RuntimeCharacter> enemies)
        {
            return BattleManager.current.CardController.DeckPile.Cards.Count;
        }
    }
}