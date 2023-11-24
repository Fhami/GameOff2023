using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BuffIcon : MonoBehaviour
{
    [SerializeField] Image _buff_img;
    [SerializeField] TextMeshProUGUI _buff_number_img;
    [SerializeField] GameObject _positive_buff_efx;
    [SerializeField] GameObject _negative_buff_efx;
    [SerializeField] Color _neutralColor;
    [SerializeField] Color _positiveColor;
    [SerializeField] Color _negativeColor;

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
            _positive_buff_efx?.gameObject.SetActive(true);
            _negative_buff_efx?.gameObject.SetActive(false);
        }
        else if(type == BuffType.Negative)
        {
            _buff_number_img.color = _negativeColor;
            _positive_buff_efx?.gameObject.SetActive(false);
            _negative_buff_efx?.gameObject.SetActive(true);
        }
        else
        {
            _buff_number_img.color = _neutralColor;
            _positive_buff_efx?.gameObject.SetActive(false);
            _negative_buff_efx?.gameObject.SetActive(false);
        }

     }
}
