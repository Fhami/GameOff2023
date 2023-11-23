using DefaultNamespace;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class ActiveSkillIcon : MonoBehaviour
{
    [SerializeField] Button _button;
    [SerializeField] Image _dimImage;
    [SerializeField] Image _icon_img;
    [SerializeField] Image _size_img;
    [SerializeField] Color _smallColor;
    [SerializeField] Color _mediumColor;
    [SerializeField] Color _bigColor;
    Action _onClick;

    public void SetEnable(bool enable)
    {
        _dimImage.gameObject.SetActive(enable);
        _button.interactable = enable;
    }

    public void SetSkill(EffectData effectData, Action onClick)
    {
        this._icon_img.sprite = effectData.intent.icon;
        this._onClick = onClick;
    }

    public void OnClick()
    {
        this._onClick.Invoke();
    }

    public void SetSize(Size size)
    {
        if (size == Size.Small)
        {
            _size_img.color = _smallColor;

        }
        else if (size == Size.Medium)
        {
            _size_img.color = _mediumColor;

        }
        else
        {
            _size_img.color = _bigColor;
        }
    }
}
