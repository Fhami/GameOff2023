using DefaultNamespace;
using System.Text;
using AYellowpaper.SerializedCollections;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CardUI : MonoBehaviour, IPointerClickHandler
{
    public bool Interactable = true;
    [SerializeField] public TextMeshProUGUI CardName;
    [SerializeField] public TextMeshProUGUI EffectTextUI;
    [SerializeField] private SerializedDictionary<CardType, GameObject> visualDict;
    [SerializeField] private SerializedDictionary<Size, GameObject> borderDict;
    [SerializeField] public CardData cardData;
    public Image Image;
    
    public UnityEvent<CardUI> OnClick;
    
    public void InitCard(CardData _cardData)
    {
        cardData = _cardData;
        CardName.text = cardData.name;
        EffectTextUI.text = CardData.GetCardDescription(cardData);
        
        foreach (var _obj in visualDict.Values)
        {
            _obj.SetActive(false);
        }

        foreach (var _obj in borderDict.Values)
        {
            _obj.SetActive(false);
        }
            
        if (visualDict.TryGetValue(cardData.cardType, out var _value))
        {
            _value.SetActive(true);
        }

        if (borderDict.TryGetValue(cardData.cardSize, out var _size))
        {
            _size.SetActive(true);
        }

        Image.sprite = _cardData.cardImage;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (!Interactable) return;
        
        OnClick?.Invoke(this);
    }
}
