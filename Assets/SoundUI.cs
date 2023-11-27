using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SoundUI : MonoBehaviour
{
    [SerializeField] Canvas _main_canvas;
    [SerializeField] AudioMixer _mixer;
    [SerializeField] Slider _bgm_slider;
    [SerializeField] Slider _sfx_slider;
    [SerializeField] Slider _ui_slider;
    [SerializeField] Toggle _mute_toggle;
    bool isShow = false;

    public void Start()
    {
        
    }

    public void Show()
    {
        if (isShow) return;
        isShow = true;
    }

    public void Hide()
    {
        if (!isShow) return;
        isShow = false;
    }


    public void OnBGMChange(float value)
    {

    }

    public void OnSFXChange(float value)
    {

    }

    public void OnUIChange(float value)
    {

    }

}
