using DefaultNamespace;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EncounterData", menuName = "ScriptableObjects/EncounterData", order = 1)]
public class EncounterData : ScriptableObject
{
    public NodeType type;
    public Sprite icon;
    public List<EnemyDataModifier> enemies;
}

[System.Serializable] public class EnemyDataModifier
{
    public CharacterData baseData;
    public int minHpMod = 0;
    public int maxHpMod = 0;

}
