using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using MoreMountains.Feedbacks;
using DefaultNamespace;
using MPUIKIT;
using DG.Tweening;

public class DeadIcon : MonoBehaviour
{
    [SerializeField] Image _icon;
    [SerializeField] TextMeshProUGUI _value_txt;
    [SerializeField] GameObject alert_efx;


    public void SetValue(int value)
    {
        _value_txt.text = value.ToString();
    }

    public void Show()
    {
        _icon.gameObject.SetActive(true);
    }

    public void Hide()
    {
        _icon.gameObject.SetActive(false);
    }

    public void SetAlert(bool enable)
    {
        alert_efx.gameObject.SetActive(enable);
    }
}
