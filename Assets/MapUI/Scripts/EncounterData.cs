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
}

[System.Serializable] public class EnemyDataModifier
{
    public CharacterData baseData;
    [MinMaxSlider(-100, 100)]
    public Vector2Int minMaxHpMod;

}
