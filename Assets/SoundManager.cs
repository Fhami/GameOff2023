using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using DG.Tweening;

public class SoundManager : Singleton<SoundManager>
{
    [SerializeField] AudioSource _bgmSource;
    [SerializeField] AudioSource _sfxSource;
    [SerializeField] AudioMixer _mixer;

    public const string MASTER_KEY = "mastervol";
    public const string BGM_KEY = "bgmvol";
    public const string SFX_KEY = "sfxvol";

    public const float DEFAULT_MASTER_VOL = 0.17f;
    public const float DEFAULT_BGM_VOL = 0.7f;
    public const float DEFAULT_SFX_VOL = 0.7f;


    public AudioMixer Mixer { get => _mixer; set => _mixer = value; }

    protected override void Awake()
    {
        base.Awake();
        if (Mixer == null) Mixer = Resources.Load<AudioMixer>("AudioMixer");
        _bgmSource = this.gameObject.AddComponent<AudioSource>();
        _sfxSource = this.gameObject.AddComponent<AudioSource>();

        _bgmSource.outputAudioMixerGroup = Mixer.FindMatchingGroups("BGM")[0];
        _bgmSource.loop = true;
        _bgmSource.playOnAwake = false;

        _sfxSource.outputAudioMixerGroup = Mixer.FindMatchingGroups("SFX")[0];
        _sfxSource.loop = false;
        _sfxSource.playOnAwake = false;

        float v;
        if (PlayerPrefs.HasKey(MASTER_KEY))
        {
            v = PlayerPrefs.GetFloat(MASTER_KEY);
            _mixer.SetFloat(MASTER_KEY, Mathf.Log10(v) * 20);
            Debug.Log(v);
        }
        else
        {
            _mixer.SetFloat(MASTER_KEY, Mathf.Log10(DEFAULT_MASTER_VOL) * 20);
        }


        if (PlayerPrefs.HasKey(BGM_KEY))
        {
            v = PlayerPrefs.GetFloat(BGM_KEY);
            _mixer.SetFloat(BGM_KEY, Mathf.Log10(v) * 20);
            Debug.Log(v);
        }
        else
        {
            _mixer.SetFloat(BGM_KEY, Mathf.Log10(DEFAULT_BGM_VOL) * 20);
        }


        if (PlayerPrefs.HasKey(SFX_KEY))
        {
            v = PlayerPrefs.GetFloat(SFX_KEY);
            _mixer.SetFloat(SFX_KEY, Mathf.Log10(v) * 20);
            Debug.Log(v);
        }
        else
        {
            _mixer.SetFloat(SFX_KEY, Mathf.Log10(DEFAULT_SFX_VOL) * 20);
        }

    }

    public void PlayBGM(AudioClip audio)
    {
        _bgmSource.clip = audio;
        _bgmSource.Play();
    }

    public void PlaySFX(AudioClip audio)
    {
        _sfxSource.PlayOneShot(audio);
    }

    
}
