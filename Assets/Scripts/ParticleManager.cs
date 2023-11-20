using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleManager : MonoBehaviour
{
    public static ParticleManager Instance;
    
    private void Awake()
    {
        Instance = this;
        DontDestroyOnLoad(this);
    }
    
    
}
