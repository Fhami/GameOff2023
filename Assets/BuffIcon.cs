using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BuffIcon : MonoBehaviour
{
    [SerializeField] Image _buff_img;
    [SerializeField] TextMeshProUGUI _buff_number_img;

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
}
