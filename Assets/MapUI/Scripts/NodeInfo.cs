using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum NodeType { Minor, Elite, Boss, Shop, Treasure, Event, Start, Rest, Artifact}

[CreateAssetMenu(fileName = "NodeInfo", menuName = "ScriptableObjects/NodeInfo", order = 1)]
public class NodeInfo : ScriptableObject
{
    public NodeType nodeType;
    public Sprite icon;
    
}
