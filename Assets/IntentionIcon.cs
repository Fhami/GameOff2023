using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class IntentionIcon : MonoBehaviour
{

    [SerializeField] Image _icon_img;
    [SerializeField] TextMeshProUGUI _value_txt;

    public void SetIcon(Sprite sprite)
    {
        _icon_img.sprite = sprite;
    }

    public void SetValue(int value)
    {
        _value_txt.text = value.ToString();
    }

}
