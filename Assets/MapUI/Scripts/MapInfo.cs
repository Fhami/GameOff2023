using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using AYellowpaper.SerializedCollections;

[CreateAssetMenu(fileName = "MapInfo", menuName = "ScriptableObjects/MapInfo", order = 1)]
public class MapInfo : ScriptableObject
{
    
    public string mapName;
    public string note;
    public List<RowSetting> rows;
    
}

[Serializable] public class RowSetting
{
    public int minNode;
    public int maxNode;
    //public List<NodeInfo> possibleNode;
    [SerializedDictionary("NodeType", "Chance")]
    public List<NodeInfo> fixedNodes;
    public SerializedDictionary<NodeInfo, int> possibleNodes;
    //public List<EncounterData> possibleEncounters;
    public SerializedDictionary<NodeType, List<EncounterData>> possibleEncounters;
    //public List<NodeType>
}


