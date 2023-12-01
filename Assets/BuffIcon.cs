using System.Collections;
using System.Collections.Generic;
using DefaultNamespace;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class BuffIcon : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] Image _buff_img;
    [SerializeField] TextMeshProUGUI _buff_number_img;
    [SerializeField] GameObject _positive_buff_efx;
    [SerializeField] GameObject _negative_buff_efx;
    [SerializeField] Color _neutralColor;
    [SerializeField] Color _positiveColor;
    [SerializeField] Color _negativeColor;
    public BuffData buffData;
    public RuntimeCharacter Character;
    public Image Buff_img { get => _buff_img; set => _buff_img = value; }
    public TextMeshProUGUI Buff_number_img { get => _buff_number_img; set => _buff_number_img = value; }

    public void SetValue(int value)
    {
        _buff_number_img.text = value.ToString();
    }

    public void SetImage(Sprite sprite)
    {
        _buff_img.sprite = sprite;
    }

    public void SetBuffType(BuffType type)
    {
        if(type == BuffType.Positive)
        {
            _buff_number_img.color = _positiveColor;
            if (_positive_buff_efx)
            {
                _positive_buff_efx?.gameObject.SetActive(true);
            }

            if (_negative_buff_efx)
            {
                _negative_buff_efx?.gameObject.SetActive(false);
            }
        }
        else if(type == BuffType.Negative)
        {
            _buff_number_img.color = _negativeColor;
            if (_positive_buff_efx)
            {
                _positive_buff_efx?.gameObject.SetActive(false);
            }

            if (_negative_buff_efx)
            {
                _negative_buff_efx?.gameObject.SetActive(true);
            }
        }
        else
        {
            _buff_number_img.color = _neutralColor;
            if (_positive_buff_efx)
            {
                _positive_buff_efx?.gameObject.SetActive(false);
            }

            if (_negative_buff_efx)
            {
                _negative_buff_efx?.gameObject.SetActive(false);
            }
        }

     }

    public void OnPointerEnter(PointerEventData eventData)
    {
        BattleManager.current.TooltipUI.Show(buffData.name, buffData.GetDescriptionWithModifier(Character), transform.position, TooltipUI.Side.TopRight);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        BattleManager.current.TooltipUI.Hide();
    }
}
