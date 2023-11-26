using System;
using System.Collections;
using System.Collections.Generic;
using DefaultNamespace;
using UnityEngine;

[CreateAssetMenu(menuName = "Gamejam/Effect/Redraw Effect")]
public class RedrawEffect : EffectData
{
    public int count = 1;
    
    public override IEnumerator Execute(RuntimeCard card, RuntimeCharacter characterPlayingTheCard, RuntimeCharacter player,
        RuntimeCharacter cardTarget, List<RuntimeCharacter> enemies)
    {
        if (characterPlayingTheCard.properties.Get<bool>(PropertyKey.CANNOT_DRAW_ADDITIONAL_CARDS_CURRENT_TURN).Value)
        {
            // TODO: VFX / visuals what happens when can't draw?
            yield break;
        }
        
        yield return BattleManager.current.WaitForSelectCardToShuffle(count, player, enemies);
    }

    public override string GetDescriptionTextWithModifiers(RuntimeCard card, RuntimeCharacter characterPlayingTheCard,
        RuntimeCharacter player, RuntimeCharacter cardTarget, List<RuntimeCharacter> enemies)
    {
        return GetDescriptionText();
    }

    public override string GetDescriptionText()
    {
        return $"Discard {count} cards and draw the same amount.";
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
