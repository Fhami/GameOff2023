using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
[CreateAssetMenu(fileName = "MapInfo", menuName = "ScriptableObjects/MapInfo", order = 1)]
public class MapInfo : ScriptableObject
{
    public int rowAmount = 3;
    public List<RowSetting> rows;
    
}

[Serializable] public class RowSetting
{
    public int minNode;
    public int maxNode;
    public List<NodeInfo> possibleNode;
    //public List<NodeType>
}

