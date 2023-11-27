using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DefaultNamespace;
using UnityEngine.EventSystems;
using MoreMountains.Feedbacks;

public class IntentionIcon : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] CanvasGroup _main_canvas;
    [SerializeField] Image _balloon_img;
    [SerializeField] Image _icon_img;
    [SerializeField] GameObject _vfx_icon;
    [SerializeField] TextMeshProUGUI _value_txt;

    [Header("Multiplier")]
    [SerializeField] Image _multiplier_img;
    [SerializeField] TextMeshProUGUI _multiplier_txt;
    [SerializeField] ParticleSystem _focus_efx;

    [SerializeField] ParticleSystem _small_size_efx;
    [SerializeField] ParticleSystem _big_size_efx;

    public IntentionDetail IntentionDetail;
    public RuntimeCharacter Character;

    /// <summary>
    /// Use if icon is VFX
    /// </summary>
    /// <param name="gob"> Reference VFX gameobject</param>
    public void SetIcon(GameObject gob)
    {
        if (_vfx_icon != null) GameObject.Destroy(_vfx_icon);
        _vfx_icon = Instantiate(gob);
        _vfx_icon.transform.SetParent(_balloon_img.transform);
        _vfx_icon.transform.localScale = new Vector3(1, 1, 1);
        _vfx_icon.transform.localPosition = new Vector3(0, 0, 0);
        _icon_img.gameObject.SetActive(false);
    }

    public void SetIcon(Sprite sprite)
    {
        if (_vfx_icon != null) GameObject.Destroy(_vfx_icon);
        _icon_img.sprite = sprite;
    }

    public void SetVFXIcon(GameObject refGameObject)
    {
        var o = Instantiate(refGameObject);
        o.transform.SetParent(_icon_img.transform);
        o.transform.localScale = new Vector3(1, 1, 1);
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
            _multiplier_txt.text = "X" + value.ToString();
        }
        else
        {
            _multiplier_img.gameObject.SetActive(false);
        }
    }

    public void SetSizeEffect(Size size)
    {
        if (size == Size.S)
        {
            if (_small_size_efx)
                _small_size_efx.gameObject.SetActive(true);
        }
        else if (size == Size.L)
        {
            if (_big_size_efx)
                _big_size_efx.gameObject.SetActive(true);
        }
        else
        {
            if (_small_size_efx)
                _small_size_efx.gameObject.SetActive(false);
            
            if (_big_size_efx)
                _big_size_efx.gameObject.SetActive(false);

            //new IntentionDetail(new IntentData(), 2).SetMultiplierMod(IntentionDetail.ValueMod.None).SetValueMod;
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        BattleManager.current.TooltipUI.Show(IntentionDetail._intentData.name, IntentionDetail._description, transform.position, TooltipUI.Side.TopLeft);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        BattleManager.current.TooltipUI.Hide();
    }
}
