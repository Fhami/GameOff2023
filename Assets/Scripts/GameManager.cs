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

        public DeckData PlayerDeck;
        
        private void Awake()
        {
            Instance = this;
            DontDestroyOnLoad(this);
            
            Debug.Log("Hello from GameManager!");
        }

        private void Start()
        {
            Init();
        }

        private void Init()
        {
            PlayerDeck = new DeckData();
        }
    }
}

