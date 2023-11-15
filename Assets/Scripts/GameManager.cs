using System;
using UnityEngine;

namespace DefaultNamespace
{
    /// <summary>
    /// Main manager for the game. Use for common shared functionality and tracking player data and progression.
    /// </summary>
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance;

        public RuntimeDeckData PlayerRuntimeDeck;
        
        private void Awake()
        {
            Instance = this;
            DontDestroyOnLoad(this);
            Init();
            Debug.Log("Hello from GameManager!");
        }

        private void Start()
        {
            
        }

        private void Init()
        {
            PlayerRuntimeDeck = new RuntimeDeckData();
        }
    }
}

