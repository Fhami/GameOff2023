using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using MoreMountains.Feedbacks;
using DefaultNamespace;
using MPUIKIT;
using DG.Tweening;
using AYellowpaper.SerializedCollections;

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

    [SerializeField] DeadIcon _big_dead_icon;
    [SerializeField] DeadIcon _small_dead_icon;


    [SerializeField] MMF_Player text_squash;
    [SerializeField] MMF_Player reduce_squash;

    [Header("Effect")]
    [SerializeField] ParticleSystem _increase_efx;
    [SerializeField] ParticleSystem _decrease_efx;

    [Header("Tag")]
    [SerializeField] TagSizeUI _sizeTag_prefab;
    [SerializeField] GameObject _sizeSeparator_prefab;
    [SerializeField] Transform _line_parent;

    [Header("Fill Graphic")]
    [SerializeField] MPImage _line;
    [SerializeField] MPImage _form;
    [SerializeField] MPImage _size;

    [Header("Color")]
    [SerializeField] Color _s_color;
    [SerializeField] Color _m_color;
    [SerializeField] Color _l_color;


    [Header("Cache")]
    [SerializeField] List<GameObject> _lines = new List<GameObject>();
    [SerializeField] SerializedDictionary<int, TagSizeUI> _tagUIs = new SerializedDictionary<int, TagSizeUI>();

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


    public void InitSizeUI(int start, SizeSetting _setting)
    {
        foreach(var o in _lines)
        {
            GameObject.Destroy(o);
        }
        _lines.Clear();

        foreach (var o in _tagUIs)
        {
            GameObject.Destroy(o.Value.gameObject);
        }
        _tagUIs.Clear();


        InitSizeUI(start, _setting.s, _setting.l);

        if (_setting.minDead != -1)
        {
            _big_dead_icon.SetValue(_setting.minDead);
            _small_dead_icon.Show();
        }
        else _small_dead_icon.Hide();

        if (_setting.maxDead != -1)
        {
            _big_dead_icon.SetValue(_setting.maxDead);
            _big_dead_icon.Show();
        }
        else _big_dead_icon.Hide();


        SetSizeSaperation(_setting.s, _setting);
        SetSizeSaperation(_setting.l, _setting);

        foreach(var skill in _setting.skills)
        {
            SetTag(skill, _setting);
        }


        GoToSize(0, start, _setting);

    }


    public void InitSizeUI(int startSize, int smallSize, int bigSize, int smallDeadSize, int bigDeadSize)//OLD
    {
        InitSizeUI(startSize, smallSize, bigSize);

        if (smallDeadSize > -1) SetSize(startSize);
        else _small_dead_icon.gameObject.SetActive(false);

        if (bigDeadSize > -1) SetSize(startSize);
        else _big_dead_icon.gameObject.SetActive(false);
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

    public void SetTag(int value, SizeSetting setting)
    {
        var t = Instantiate(_sizeTag_prefab);
        t.transform.SetParent(main_canvas.transform);
        t.transform.localScale = new Vector3(1, 1, 1);
        t.transform.localPosition = new Vector3(0, 0, 0);
        var percent = (float)value / (float)setting.max;
        var total = -(percent * 180);
        t.transform.localRotation = Quaternion.Euler(0, 0, total);
        t.Set(setting.GetSize(value), value);
        _tagUIs.Add(value, t); 
        Debug.Log(total);
    }

    public void SetSizeSaperation(int value, SizeSetting setting)
    {

        var t = Instantiate(_sizeSeparator_prefab);
        t.transform.SetParent(main_canvas.transform);
        t.transform.localScale = new Vector3(1, 1, 1);
        t.transform.localPosition = new Vector3(0, 0, 0);
        var percent = (float)value / (float)setting.max;
        var total = -(percent * 180);
        t.transform.localRotation = Quaternion.Euler(0, 0, total);
        t.transform.SetParent(_line_parent);
        _lines.Add(t);
    }

    Tween _sizeNumberTween;
    Sequence _sizeTween;

    public void GoToSize(int from,int to, SizeSetting setting, System.Action onComplete = null)
    {
        if (_sizeTween != null && _sizeTween.IsPlaying())
        {
            _sizeTween.Kill(true);
        }
  
        _sizeTween = DOTween.Sequence();

        var percent = ((float)to / (float)setting.max) * 0.5f;
        var form_delay = 0f;
        var size_delay = 0f;
        if (Mathf.Abs(from - to) >= 2) form_delay = 0.3f;
        if ((from - to) < 0)
        {
            size_delay = 0.3f;
        }
        else
        {
            form_delay = 0;
        }

        _sizeTween.Join(DOTween.To(() => from, x => from = x, to, 0.2f).
             OnUpdate(() => {
                 _current_size_txt.text = from.ToString();
            }));

        _sizeTween.Join(_line.DOFillAmount(percent + 0.005f, 0.3f));
        _sizeTween.Join(_form.DOFillAmount(percent - 0.005f, 0.3f).SetDelay(form_delay));

        if(setting.GetSize(to) == Size.L)
        {
            _form.DOColor(_l_color,0.2f);
        }
        else if (setting.GetSize(to) == Size.S)
        {
            _form.DOColor(_s_color, 0.2f);
        }
        else
        {
            _form.DOColor(_m_color, 0.2f);
        }


        if (to > setting.l)
        {
            var L = (float)setting.l / (float)setting.max;
            _sizeTween.Join(_size.DOFillAmount(L * 0.5f, 0.2f).SetDelay(size_delay));
        }
        //else if (value >= m)
        //{
        //    var M = (float)m / (float)maxSize;
        //    _size.DOFillAmount(M * 0.5f, 0.2f).SetDelay(0.3f);
        //}
        else if(to > setting.s)
        {
            var S = (float)setting.s / (float)setting.max;
            _sizeTween.Join(_size.DOFillAmount(S * 0.5f, 0.2f).SetDelay(size_delay));
        }
        else
        {
            _sizeTween.Join(_size.DOFillAmount(0, 0.2f).SetDelay(size_delay));
        }


        if (setting.maxDead != -1)
        {
            if (to >= setting.maxDead) SetDangerMax(true);
            else SetDangerMax(false);
        }

        if (setting.minDead != -1)
        {
            if (to <= setting.minDead) SetDangerMin(true);
            else SetDangerMin(false);
        }


        _sizeTween.OnComplete(() =>
        {
            Debug.Log("Complete");
            
            onComplete?.Invoke();
        });

        //_sizeTween.Play();
    }

    public void SetDangerMin(bool value)
    {
        _small_dead_icon.SetAlert(value);
    }

    public void SetDangerMax(bool value)
    {
        _big_dead_icon.SetAlert(value);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="tagID"> Tag ID</param>
    /// <param name="value"></param>
    public void HighlightTag(int tagID, bool value)
    {
        if(_tagUIs.TryGetValue(tagID,out var s))
        {
            s.Highlight(value);
        }
    }
}

[System.Serializable] public class SizeSetting
{
    public int s;
    public int m;
    public int l;
    public int min;
    public int max;
    public int minDead;
    public int maxDead;
    public List<int> skills;

    public SizeSetting ( int s, int m,int l, int min, int max, int minDead, int maxDead)
    {
        this.s = s;
        this.m = m;
        this.l = l;
        this.min = min;
        this.max = max;
        this.minDead = minDead;
        this.maxDead = maxDead;
    }

    public Size GetSize(int value)
    {
        if (value >= l)
        {
            return Size.L;
        }
        else if (value <= s)
        {
            return Size.S;
        }
        else
        {
            return Size.M;
        }
    }
}
