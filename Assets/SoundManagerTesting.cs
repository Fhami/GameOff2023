using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SoundType { Map, Normal, Elite, Boss}
public class SoundManagerTesting : MonoBehaviour
{
    [SerializeField] AudioClip clip;

    [SerializeField] AudioClip mapBgm;
    [SerializeField] AudioClip normalBgm;
    [SerializeField] AudioClip eliteBgm;
    [SerializeField] AudioClip bossBgm;

    // Start is called before the first frame update
    void Start()
    {
        SoundManager.Instance.PlayBGM(normalBgm);
    }

    public void PlayBGM(SoundType type)
    {
        if(type == SoundType.Map)
        {
            SoundManager.Instance.PlayBGM(mapBgm);
        }
        else if (type == SoundType.Normal)
        {
            SoundManager.Instance.PlayBGM(normalBgm);
        }
        else if (type == SoundType.Elite)
        {
            SoundManager.Instance.PlayBGM(eliteBgm);
        }
        else if (type == SoundType.Boss)
        {
            SoundManager.Instance.PlayBGM(bossBgm);
        }
    }

    //public void BGM_Play01()
    //{
    //    PlayBGM(SoundType.Elite);
    //}

    //public void BGM_Play02()
    //{
    //    PlayBGM(SoundType.Boss);
    //}
}
