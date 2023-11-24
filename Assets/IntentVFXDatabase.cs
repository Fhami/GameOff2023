using AYellowpaper.SerializedCollections;
using System.Collections;
using DefaultNamespace;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "IntentVFXDatabase", menuName = "ScriptableObjects/IntentVFXDatabase", order = 1)]
public class IntentVFXDatabase : ScriptableObject
{
    public SerializedDictionary<IntentData, GameObject> _intentVFXs;
}
