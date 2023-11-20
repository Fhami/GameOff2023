using DefaultNamespace;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GenerateCardScript : MonoBehaviour
{
    public CardData[] Card;
    public CardData[] CardList;
    public List <CardUI> CurrentCard;


    public int cardMAX = 3;
    public int cardType = new int();
    
    void Start()
    {
    }

    void Update()
    {
        
    }

    public void GenerateCard()
    {
        CardUI cardUI;

        Card = CardList;

        
        foreach (var card in CurrentCard)
        {
            Destroy(card.gameObject);
        }
        CurrentCard.Clear();

        
        for (int i = 0; i < cardMAX; i++)
        {
            
            cardType = Random.Range(0, Card.Length);
            cardUI = CardFactory.CreateCardUI(Card[cardType].name);
            cardUI.transform.SetParent(transform, false);
            CurrentCard.Add(cardUI);

            RemoveSpawnedPrefab(cardType);
            
        }

    }

    private void RemoveSpawnedPrefab(int index)
    {
        List<CardData> prefabList = new List<CardData>(Card);
        prefabList.RemoveAt(index);
        Card = prefabList.ToArray();
    }

}
