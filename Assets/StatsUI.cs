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
    [Header("HP")]
    [SerializeField] Image _hp_img;
    [SerializeField] TextMeshProUGUI _hp_txt;


    [Header("HP FEEDBACK")]
    [SerializeField] MMF_Player _hp_decrease_feedback;
    [SerializeField] MMF_Player _hp_increase_feedback;
    [SerializeField] ParticleSystem _hp_increase_efx;
    [SerializeField] ParticleSystem _hp_decrease_efx;
    Tween _hpNumberTween;
    Tween _hpBarTween;

    [Header("BUFF")]
    [SerializeField] BuffIcon _buff_prefab;
    [SerializeField] GameObject _buff_content;
    [SerializeField] SerializedDictionary<BuffData, BuffIcon> _buffs;

    public void SetHp(int from, int to,int max, float duration, System.Action onComplete = null)
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
            .OnComplete(() => {
                onComplete?.Invoke();
            })
            .OnUpdate(()=> {
                _hp_txt.text = from.ToString() +"/"+ max.ToString();
            });

        //Animate Hp bar

        _hpBarTween = _hp_img.DOFillAmount(targetPercentile, duration);

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

    #endregion

}
