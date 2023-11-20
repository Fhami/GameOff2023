using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using MoreMountains.Feedbacks;

public enum SizeEffectType { Increase,Decrease}
public class SizeUI : MonoBehaviour
{

    [SerializeField] CanvasGroup main_canvas; 
    [SerializeField] TextMeshProUGUI _current_size_txt;
    [SerializeField] TextMeshProUGUI _big_size_txt;
    [SerializeField] TextMeshProUGUI _small_size_txt;
    [SerializeField] Image _big_img;
    [SerializeField] Image _small_img;

    [SerializeField] MMF_Player text_squash;

    [Header("Effect")]
    [SerializeField] ParticleSystem _increase_efx;
    [SerializeField] ParticleSystem _decrease_efx;

   /// <summary>
   /// Use this to init this UI
   /// </summary>
   /// <param name="startSize"></param>
   /// <param name="smallSize"></param>
   /// <param name="bigSize"></param>
    public void InitSizeUI(int startSize, int smallSize, int bigSize)
    {
        SetSize(startSize);

        if (smallSize > -1) SetSize(smallSize);
        else SetEnableSmall(false);

        if (bigSize > -1) SetSize(bigSize);
        else SetEnableBig(false);
    }

    protected void SetSize(int size)
    {
        _current_size_txt.text = size.ToString();
    }

    public void SetSize(int size, SizeEffectType effect)
    {
        SetSize(size);
        text_squash.PlayFeedbacks();
        //if(effect == SizeEffectType.Decrease)
        //{
        //    _decrease_efx.Play();
        //}
        //else
        //{
        //    _increase_efx.Play();
        //}
    }

    public void SetEnableBig(bool enable)
    {
        _big_img.gameObject.SetActive(enable);
    }

    public void SetEnableSmall(bool enable)
    {
        _small_img.gameObject.SetActive(enable);
    }

    public void SetBigSize(int size)
    {
        _big_size_txt.text = size.ToString();
    }

    public void SetSmallSize(int size)
    {
        _small_size_txt.text = size.ToString();
    }

    public void SetActive(bool active)
    {
        main_canvas.gameObject.SetActive(active);
    }

    #region Test Button Function

    public void Btn_Increase()
    {
        SetSize(8,SizeEffectType.Increase);
    }
    public void Btn_Decrease()
    {
        SetSize(2, SizeEffectType.Decrease);
    }
    public void Btn_Set()
    {
        SetSize(5);
    }

    #endregion
}
