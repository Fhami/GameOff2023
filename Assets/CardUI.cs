using DefaultNamespace;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CardUI : MonoBehaviour, IPointerClickHandler
{
    //[SerializeField] public TextMeshPro EffectText;
    [SerializeField] public TextMeshProUGUI CardName;
    [SerializeField] public TextMeshProUGUI EffectTextUI;

    [SerializeField] public CardData cardData;

    public Button button;
    public UnityEvent<CardUI> OnClick;

    void Start()
    {

    }

    void Update()
    {
        
    }
    public void InitCard(RuntimeCard runtimeCard)
    {
        cardData = runtimeCard.cardData;
        StringBuilder _builder = new StringBuilder();

        CardName.text = cardData.name;

        foreach(var _effect in cardData.effects)
        {
            _builder.AppendLine(_effect.GetDescriptionText());
        }

        EffectTextUI.text = _builder.ToString();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        OnClick?.Invoke(this);
    }
}
