using AYellowpaper.SerializedCollections;
using DefaultNamespace;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CardTypeColorDatabase", menuName = "ScriptableObjects/CardTypeColorDatabase", order = 1)]
public class CardTypeColorDatabase : ScriptableObject
{
    public SerializedDictionary<CardType, Color> _colorDatas;

}
