using System.Collections;
using System.Collections.Generic;
using DefaultNamespace;
using UnityEngine;

[CreateAssetMenu(menuName = "Mockup/Deck", fileName = "MockupDeckData")]
public class DeckData : ScriptableObject
{
    public List<CardData> Cards = new List<CardData>();
}
