using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManagerTesting : MonoBehaviour
{
    [SerializeField] AudioClip clip;

    // Start is called before the first frame update
    void Start()
    {
        SoundManager.Instance.PlayBGM(clip);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
