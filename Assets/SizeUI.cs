using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using MoreMountains.Feedbacks;
using DefaultNamespace;
using MPUIKIT;
using DG.Tweening;

public enum SizeEffectType { Increase,Decrease}
public class SizeUI : MonoBehaviour
{

    [SerializeField] CanvasGroup main_canvas; 
    [SerializeField] TextMeshProUGUI _current_size_txt;
    [SerializeField] TextMeshProUGUI _big_size_txt;
    [SerializeField] TextMeshProUGUI _small_size_txt;
    [SerializeField] TextMeshProUGUI _big_dead_size_txt;
    [SerializeField] TextMeshProUGUI _small_dead_size_txt;

    [SerializeField] Image _big_img;
    [SerializeField] Image _small_img;
    [SerializeField] Image _big_dead_img;
    [SerializeField] Image _small_dead_img;


    [SerializeField] MMF_Player text_squash;
    [SerializeField] MMF_Player reduce_squash;

    [Header("Effect")]
    [SerializeField] ParticleSystem _increase_efx;
    [SerializeField] ParticleSystem _decrease_efx;

    [Header("Tag")]
    [SerializeField] TagSizeUI _sizeTag_prefab;
    [SerializeField] GameObject _sizeSeparator_prefab;

    [Header("Fill Graphic")]
    [SerializeField] MPImage _line;
    [SerializeField] MPImage _form;
    [SerializeField] MPImage _size;


    /// <summary>
    /// Use this to init this UI
    /// </summary>
    /// <param name="startSize"></param>
    /// <param name="smallSize"></param>
    /// <param name="bigSize"></param>
    public void InitSizeUI(int startSize, int smallSize, int bigSize)
    {
        SetSize(startSize);

        if (smallSize > -1) SetSmallSize(smallSize);
        else SetEnableSmall(false);

        if (bigSize > -1) SetBigSize(bigSize);
        else SetEnableBig(false);
    }

    public void InitSizeUI(int startSize, int smallSize, int bigSize, int smallDeadSize, int bigDeadSize)
    {
        InitSizeUI(startSize, smallSize, bigSize);

        if (smallDeadSize > -1) SetSize(smallDeadSize);
        else _small_dead_img.gameObject.SetActive(false);

        if (bigDeadSize > -1) SetSize(bigDeadSize);
        else _big_dead_img.gameObject.SetActive(false);
    }

    public void SetSize(int size)
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

    public void SetTag(Size size, int value, int minSize, int maxSize)
    {
        var t = Instantiate(_sizeTag_prefab);
        t.transform.SetParent(main_canvas.transform);
        t.transform.localScale = new Vector3(1, 1, 1);
        t.transform.localPosition = new Vector3(0, 0, 0);
        var percent = (float)value / (float)maxSize;
        var total = -(percent * 180);
        t.transform.localRotation = Quaternion.Euler(0, 0, total);
        t.Set(size, value);
        Debug.Log(total);
    }

    public void SetSizeSaperation(int value, int minSize, int maxSize)
    {

        var t = Instantiate(_sizeSeparator_prefab);
        t.transform.SetParent(main_canvas.transform);
        t.transform.localScale = new Vector3(1, 1, 1);
        t.transform.localPosition = new Vector3(0, 0, 0);
        var percent = (float)value / (float)maxSize;
        var total = -(percent * 180);
        t.transform.localRotation = Quaternion.Euler(0, 0, total);
    }

    public void GoToSize(int value, int s, int m, int l, int minSize, int maxSize)
    {
        var percent = ((float)value / (float)maxSize) * 0.5f;
        _size.DOFillAmount(percent,0.2f);
        _form.DOFillAmount(percent, 0.2f);
        _line.DOFillAmount(percent, 0.2f);

    }

}
