using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SoundUI : MonoBehaviour
{
    [SerializeField] CanvasGroup _main_canvas;
    [SerializeField] AudioMixer _mixer;
    [SerializeField] Slider _bgm_slider;
    [SerializeField] Slider _sfx_slider;
    [SerializeField] Slider _main_slider;
    [SerializeField] Toggle _mute_toggle;

    [SerializeField] float _masterLevel;
    [SerializeField] float _bgmLevel;
    [SerializeField] float _sfxLevel;

    [SerializeField] bool isShow = false;

    public void Awake()
    {
        _bgm_slider.onValueChanged.AddListener(OnBGMChange);
        _sfx_slider.onValueChanged.AddListener(OnSFXChange);
        _main_slider.onValueChanged.AddListener(OnMasterChange);
    }

    public void Show()
    {
        if (isShow) return;
        isShow = true;
        _main_canvas.gameObject.SetActive(true);
        _mixer = SoundManager.Instance.Mixer;

        if (PlayerPrefs.HasKey(SoundManager.MASTER_KEY))
        {
            _masterLevel = PlayerPrefs.GetFloat(SoundManager.MASTER_KEY);
        }

        if (PlayerPrefs.HasKey(SoundManager.BGM_KEY))
        {
            _bgmLevel = PlayerPrefs.GetFloat(SoundManager.BGM_KEY);
        }

        if (PlayerPrefs.HasKey(SoundManager.SFX_KEY))
        {
            _sfxLevel = PlayerPrefs.GetFloat(SoundManager.SFX_KEY);
        }

        _main_slider.value = _masterLevel;
        _bgm_slider.value = _bgmLevel;
        _sfx_slider.value = _sfxLevel;
    }

    public void Hide()
    {
        if (!isShow) return;
        isShow = false;
        _main_canvas.gameObject.SetActive(false);
    }

    public void OnMasterChange(float value)
    {
        _masterLevel = value;
        _mixer.SetFloat(SoundManager.MASTER_KEY, Mathf.Log10(value) * 20);
    }

    public void OnBGMChange(float value)
    {
        _bgmLevel = value ;
        _mixer.SetFloat(SoundManager.BGM_KEY, Mathf.Log10(value) * 20);
    }

    public void OnSFXChange(float value)
    {
        _sfxLevel = value;
        _mixer.SetFloat(SoundManager.SFX_KEY, Mathf.Log10(value) * 20);
    }

    public void Btn_Confirm()
    {
        PlayerPrefs.SetFloat(SoundManager.MASTER_KEY, _masterLevel);
        PlayerPrefs.SetFloat(SoundManager.BGM_KEY, _bgmLevel);
        PlayerPrefs.SetFloat(SoundManager.SFX_KEY, _sfxLevel);
        Hide();
    }


}
