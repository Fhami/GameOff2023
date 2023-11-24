using AYellowpaper.SerializedCollections;
using DefaultNamespace;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BuffTypeDatabase", menuName = "ScriptableObjects/BuffTypeDatabase", order = 1)]
public class BuffTypeDatabase : ScriptableObject
{
    public SerializedDictionary<BuffData, BuffType> _buffTypes;

}
