using DefaultNamespace;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class RewardController : MonoBehaviour
{
    public List <CardUI> CurrentCard;
    public Transform cardContainer;
    public int cardMAX = 3;

    public Button SkipButton;
    public Button NextButton;

    public void GenerateCards(List<CardData> _pool)
    {
        SkipButton.gameObject.SetActive(true);
        NextButton.gameObject.SetActive(false);
        
        foreach (var _card in CurrentCard)
        {
            Destroy(_card.gameObject);
        }
        
        CurrentCard.Clear();

        foreach (var _index in GenerateRandomNumbers(cardMAX, 1, _pool.Count))
        {
            var _cardUI = CardFactory.CreateCardUI(_pool[_index]);
            _cardUI.transform.SetParent(cardContainer, false);
            _cardUI.OnClick.AddListener(AddCardToDeck);
            
            CurrentCard.Add(_cardUI);
        }
        
    }

    private void AddCardToDeck(CardUI cardUI)
    {
        foreach (var _card in CurrentCard)
        {
            _card.Interactable = false;
            if (_card != cardUI)
            {
                _card.transform.DOScale(Vector3.zero, 0.3f);
            }
        }
        GameManager.Instance.PlayerRuntimeDeck.AddCard(cardUI.cardData);
        CurrentCard.Remove(cardUI);
        //Destroy(cardUI.gameObject);
        SkipButton.gameObject.SetActive(false);
        NextButton.gameObject.SetActive(true);
        Debug.Log($"Added {cardUI.cardData.name}");
    }

    public static List<int> GenerateRandomNumbers(int count, int minValue, int maxValue)
    {
        List<int> possibleNumbers = new List<int>();
        List<int> chosenNumbers = new List<int>();
 
        for (int index = minValue; index < maxValue; index++)
            possibleNumbers.Add(index);
       
        while (chosenNumbers.Count < count)
        {
            int position = Random.Range(0, possibleNumbers.Count);
            chosenNumbers.Add(possibleNumbers[position]);
            possibleNumbers.RemoveAt(position);
        }
        return chosenNumbers;
    }
    
}
