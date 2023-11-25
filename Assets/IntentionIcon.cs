using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DefaultNamespace;
using UnityEngine.EventSystems;

public class IntentionIcon : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] Image _icon_img;
    [SerializeField] TextMeshProUGUI _value_txt;
    [Header("Multiplier")]
    [SerializeField] Image _multiplier_img;
    [SerializeField] TextMeshProUGUI _multiplier_txt;
    [SerializeField] ParticleSystem _focus_efx;

    [SerializeField] ParticleSystem _small_size_efx;
    [SerializeField] ParticleSystem _big_size_efx;

    public IntentionDetail IntentionDetail;
    public RuntimeCharacter Character;
    
    public void SetIcon(Sprite sprite)
    {
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
            _multiplier_txt.text = value.ToString();
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
