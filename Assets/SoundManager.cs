using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class SoundManager : Singleton<SoundManager>
{
    [SerializeField] AudioSource _bgmSource;
    [SerializeField] AudioSource _sfxSource;
    [SerializeField] AudioSource _uiSource;
    [SerializeField] AudioMixer _mixer;

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

        _uiSource.outputAudioMixerGroup = Mixer.FindMatchingGroups("UI")[0];
        _uiSource.loop = false;
        _uiSource.playOnAwake = false;

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

    public void PlayUISFX(AudioClip audio)
    {
        _uiSource.PlayOneShot(audio);
    }

}
