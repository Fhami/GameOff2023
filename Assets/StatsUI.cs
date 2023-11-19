using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoreMountains.Feedbacks;
using TMPro;
using UnityEngine.UI;
using AYellowpaper.SerializedCollections;
using DG.Tweening;
using DefaultNamespace;

public class StatsUI : MonoBehaviour
{
    [Header("Icon")]
    [SerializeField] GameObject _icon_content;
    [SerializeField] Image _hearth_img;
    [SerializeField] Image _sheild_img;
    [SerializeField] GameObject _preview_content;
    [SerializeField] Image _preview_sheild_img;

    [Header("HP")]
    [SerializeField] Image _hp_img;
    [SerializeField] TextMeshProUGUI _hp_txt;

    [Header("HP FEEDBACK")]
    [SerializeField] MMF_Player _hp_decrease_feedback;
    [SerializeField] MMF_Player _hp_increase_feedback;
    [SerializeField] MMF_Player _hp_preview_feedback;
    [SerializeField] ParticleSystem _hp_increase_efx;
    [SerializeField] ParticleSystem _hp_decrease_efx;
    [SerializeField] ParticleSystem _hp_focus_efx;
    Tween _hpNumberTween;
    Tween _hpBarTween;

    [SerializeField] GameObject _hp_front_preview;
    [SerializeField] Slider _hp_preview_front_slider;
    [SerializeField] Image _hp_preview_front_slider_area;


    [SerializeField] GameObject _hp_back_preview;
    [SerializeField] Image _hp_preview_increase_img;

    [Header("SHIELD")]
    [SerializeField] TextMeshProUGUI _shield_txt;
    [SerializeField] TextMeshProUGUI _preview_shield_txt;


    [Header("SHIELD FEEDBACK")]
    [SerializeField] ParticleSystem _shield_increase_efx;
    [SerializeField] ParticleSystem _shield_decrease_efx;
    [SerializeField] ParticleSystem _shield_focus_efx;
    [SerializeField] MMF_Player _shield_preview_feedback;


    [Header("BUFF")]
    [SerializeField] BuffIcon _buff_prefab;
    [SerializeField] GameObject _buff_content;
    [SerializeField] SerializedDictionary<BuffData, BuffIcon> _buffs;


    private void Start()
    {
        _hp_front_preview.SetActive(false);
        _hp_back_preview.SetActive(false);
    }

    public void SetHp(int from, int to, int max, float duration = 0.33f,
        System.Action onStart= null,  System.Action onComplete = null)
    {
        float targetPercentile = (float)to / (float)max;

        if (_hpNumberTween!=null && _hpNumberTween.IsPlaying())
        {
            _hpNumberTween.Kill(true);
        }

        if (_hpBarTween != null && _hpBarTween.IsPlaying())
        {
            _hpBarTween.Kill(true);
        }

        _hpNumberTween = DOTween.To(() => from, x => from = x, to, duration)
            .OnStart(()=> {
                onStart?.Invoke();
            })
            .OnComplete(() => {
                onComplete?.Invoke();
            })
            .OnUpdate(()=> {
                _hp_txt.text = from.ToString() +"/"+ max.ToString();
            });

        //Animate Hp bar

        _hpBarTween = _hp_img.DOFillAmount(targetPercentile, duration);

    }

    public void PlayHpUpFeedback()
    {

    }

    public void PlayHpDownFeedback()
    {

    }
    
    public void PreviewHp(int current, int to, int max)
    {
        var deltaHp = current - to;
        float currentPercentile = (float)current / (float)max;
        float deltaPercentile = (float)deltaHp / (float)max;

        if (deltaHp > 0)
        {
            //Decrease
            //Set front preview = to;
            //Set Back preview = current;
            _hp_preview_front_slider.value = currentPercentile - deltaPercentile;
            _hp_preview_front_slider_area.fillAmount = currentPercentile;

            _hp_front_preview.SetActive(true);
            _hp_back_preview.SetActive(false);


        }
        else
        {
            //Increse
            _hp_front_preview.SetActive(false);
            _hp_back_preview.SetActive(true);
            _hp_preview_increase_img.fillAmount = currentPercentile - deltaPercentile;
        }

    }

    //void AnimateHpBar(int to,float duration)
    //{
    //    _hpBarTween = _hp_img.DOFillAmount(to, duration);
    //}


    //May need to change buff ID to something elsee????? ?
    
    public void SetBuff(BuffData buffData, int value)
    {
        BuffIcon icon;

        if (!_buffs.TryGetValue(buffData, out icon)) //If not found this buff id in Dictionary, create one and add to the UI and Dictionary
        {
            icon = Instantiate<BuffIcon>(_buff_prefab);
            icon.transform.SetParent(_buff_content.transform);
            icon.transform.localScale = new Vector3(1, 1, 1);
            icon.SetImage(buffData.icon);
            _buffs.Add(buffData, icon);
        }

        if (value > 0)
        {
            icon.SetValue(value);
        }
        else//Remove this buff
        {
            Destroy(icon.gameObject);
            _buffs.Remove(buffData);
        }
    }

    public void SetShield(int to, System.Action onStart = null, System.Action onComplete = null)
    {
        onStart?.Invoke();

        if (to > 0)
        {
            _hearth_img?.gameObject.SetActive(false);
            _sheild_img?.gameObject.SetActive(true);
        }
        else
        {
            _hearth_img?.gameObject.SetActive(true);
            _sheild_img?.gameObject.SetActive(false);
        }

        _shield_txt.text = to.ToString();
        onComplete?.Invoke();
    }

    public void PreviewShield(int value)
    {
        _preview_content.SetActive(true);
        _icon_content.SetActive(false);
        _preview_shield_txt.text = value.ToString();
    }

    public void CancelPreviewShield()
    {
        _preview_content.SetActive(true);
        _icon_content.SetActive(false);
        _preview_shield_txt.text = "";
    }

    public void PlayShieldUpFeedback()
    {
       // _shield_increase_efx.Play();
    }

    public void PlayShieldDownFeedback()
    {
        // _shield_decrease_efx.Play();
    }


    #region Test Function

    public void Btn_SetHp(int hp)
    {
        SetHp(100, hp, 100,0.2f,()=> { Debug.Log("Finish set hp"); });
    }

    public void Btn_DecreaseHp()
    {
        SetHp(100, 70, 100, 0.2f);
    }

    public void Btn_AddBuffA(BuffData buffData)
    {
        SetBuff(buffData, 1);
    }

    public void Btn_RemoveBuffA(BuffData buffData)
    {
        SetBuff(buffData, 0);
    }

    public void Btn_AddBuffB(BuffData buffData)
    {
        SetBuff(buffData, 2);
    }

    public void Btn_RemoveBuffB(BuffData buffData)
    {
        SetBuff(buffData, 0);
    }

    public void Btn_GetShield(int value)
    {
        SetShield(value, PlayShieldUpFeedback);
    }

    public void Btn_LostShield(int value)
    {
        SetShield(value, PlayShieldDownFeedback);
    }

    public void Btn_Preview01()
    {
        PreviewHp(15, 3, 30);
    }

    public void Btn_Preview02()
    {
        PreviewHp(15, 27, 30);
    }

    #endregion

}
