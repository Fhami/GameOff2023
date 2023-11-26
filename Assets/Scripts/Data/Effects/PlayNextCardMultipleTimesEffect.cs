using System.Collections;
using System.Collections.Generic;
using DefaultNamespace;
using UnityEngine;

[CreateAssetMenu(menuName = "Gamejam/Effect/PlayNextCardMultipleTimesEffect")]
public class PlayNextCardMultipleTimesEffect : EffectData
{
    public int extraTimes = 1;
    public override IEnumerator Execute(RuntimeCard card, RuntimeCharacter characterPlayingTheCard, RuntimeCharacter player,
        RuntimeCharacter cardTarget, List<RuntimeCharacter> enemies)
    {
        characterPlayingTheCard.properties.Get<int>(PropertyKey.NEXT_CARD_PLAY_EXTRA_TIMES).Value += extraTimes;
        
        yield break;
    }

    public override string GetDescriptionTextWithModifiers(RuntimeCard card, RuntimeCharacter characterPlayingTheCard,
        RuntimeCharacter player, RuntimeCharacter cardTarget, List<RuntimeCharacter> enemies)
    {
        return $"Next card play {1 + extraTimes} times.";
    }

    public override string GetDescriptionText()
    {
        return $"Next card play {1 + extraTimes} times.";
    }

    public override int GetEffectValue(RuntimeCard card, RuntimeCharacter characterPlayingTheCard, RuntimeCharacter player,
        RuntimeCharacter cardTarget, List<RuntimeCharacter> enemies)
    {
        return extraTimes;
    }

    public override string GetEffectValue(RuntimeCard card = null)
    {
        return extraTimes.ToString();
    }
}
