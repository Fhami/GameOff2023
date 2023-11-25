using System.Collections;
using System.Collections.Generic;
using DefaultNamespace;
using UnityEngine;

[CreateAssetMenu(menuName = "Gamejam/Effect/PersistEffect")]
public class PersistEffect : EffectData
{
    public override IEnumerator Execute(RuntimeCard card, RuntimeCharacter characterPlayingTheCard, RuntimeCharacter player,
        RuntimeCharacter cardTarget, List<RuntimeCharacter> enemies)
    {
        yield break;
    }

    public override string GetDescriptionTextWithModifiers(RuntimeCard card, RuntimeCharacter characterPlayingTheCard,
        RuntimeCharacter player, RuntimeCharacter cardTarget, List<RuntimeCharacter> enemies)
    {
        return "Cannot be discarded.";
    }

    public override string GetDescriptionText()
    {
        return "Cannot be discarded.";
    }

    public override int GetEffectValue(RuntimeCard card, RuntimeCharacter characterPlayingTheCard, RuntimeCharacter player,
        RuntimeCharacter cardTarget, List<RuntimeCharacter> enemies)
    {
        return 0;
    }

    public override string GetEffectValue(RuntimeCard card = null)
    {
        return "";
    }

    public override int GetTimesValue(RuntimeCard card, RuntimeCharacter characterPlayingTheCard, RuntimeCharacter player,
        RuntimeCharacter cardTarget, List<RuntimeCharacter> enemies)
    {
        return 0;
    }

    public override string GetTimesValue(RuntimeCard card = null)
    {
        return "";
    }
}
