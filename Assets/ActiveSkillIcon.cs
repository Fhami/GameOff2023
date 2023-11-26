using DefaultNamespace;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using TMPro;
using UnityEngine.EventSystems;

public class ActiveSkillIcon : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] Button _button;
    [SerializeField] Image _dimImage;
    [SerializeField] Image _icon_img;
    [SerializeField] Image _size_img;
    [SerializeField] TextMeshProUGUI _size_txt;
    [SerializeField] Image _size_number_img;


    [SerializeField] Color _smallColor;
    [SerializeField] Color _mediumColor;
    [SerializeField] Color _bigColor;

    private CardData cardData;
    
    Action _onClick;

    public void SetEnable(bool enable)
    {
        _dimImage.gameObject.SetActive(!enable);
        _button.interactable = enable;
    }

    public void SetSkill(CardData cardData, Action onClick)
    {
        this.cardData = cardData;
        this._icon_img.sprite = cardData.effects[0].intent.icon;
        this._onClick = onClick;
    }

    public void OnClick()
    {
        this._onClick.Invoke();
    }

    public void SetSizeNumber(int size)
    {
        this._size_txt.text = size.ToString();
    }

    public void SetSize(Size size)
    {
        if (size == Size.S)
        {
            _size_img.color = _smallColor;
            _size_number_img.color = _smallColor;

        }
        else if (size == Size.M)
        {
            _size_img.color = _mediumColor;
            _size_number_img.color = _mediumColor;

        }
        else
        {
            _size_img.color = _bigColor;
            _size_number_img.color = _bigColor;
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        var _text = $"{CardData.GetCardDescription(cardData)}";
        BattleManager.current.TooltipUI.Show($"Size {_size_txt.text} Gain card <color=#00CED1>{cardData.name}</color>" ,_text, transform.position, TooltipUI.Side.TopRight);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        BattleManager.current.TooltipUI.Hide();
    }
}
