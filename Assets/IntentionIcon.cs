using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class IntentionIcon : MonoBehaviour
{
    public enum Size { Small, Medium, Big }

    [SerializeField] Image _icon_img;
    [SerializeField] TextMeshProUGUI _value_txt;
    [Header("Multiplier")]
    [SerializeField] Image _multiplier_img;
    [SerializeField] TextMeshProUGUI _multiplier_txt;
    [SerializeField] ParticleSystem _focus_efx;
    [SerializeField] ParticleSystem _small_size_efx;
    [SerializeField] ParticleSystem _big_size_efx;

    public void SetIcon(Sprite sprite)
    {
        _icon_img.sprite = sprite;
    }

    public void SetValue(int value)
    {
        _value_txt.text = value.ToString();
    }

    public void SetMultiplier(int value)
    {
        if (value > 1)
        {
            _multiplier_img.gameObject.SetActive(true);
            _multiplier_txt.text = value.ToString();
        }
        else
        {
            _multiplier_img.gameObject.SetActive(false);
        }
    }

    public void AddSizeEffect(IntentionDetail.Size size)
    {

    }

}
