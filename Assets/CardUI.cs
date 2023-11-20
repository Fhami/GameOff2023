using DefaultNamespace;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CardUI : MonoBehaviour
{
    //[SerializeField] public TextMeshPro EffectText;
    [SerializeField] public TextMeshProUGUI CardName;
    [SerializeField] public TextMeshProUGUI EffectTextUI;

    [SerializeField] public CardData cardData;


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

}
