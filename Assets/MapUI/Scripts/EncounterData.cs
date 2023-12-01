using DefaultNamespace;
using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EncounterData", menuName = "ScriptableObjects/EncounterData", order = 1)]
public class EncounterData : ScriptableObject
{
    public NodeType type;
    public Sprite icon;
    public List<EnemyDataModifier> enemies;
    public List<CardData> rewardPool;
    public AudioClip bgm;
}

[System.Serializable] public class EnemyDataModifier
{
    public CharacterData baseData;

    [MinMaxSlider(-100, 100)]
    public Vector2Int minMaxHpMod;
    [Min(0)] public int startHp = 0;
    [Min(0)] public int startShield = 0;
    [Min(0)]public int startSize = 5;
    public int startAttackPattern = 0;
    //Start buff or debuff.
    //

}
