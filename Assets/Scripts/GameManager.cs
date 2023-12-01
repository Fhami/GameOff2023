using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DefaultNamespace
{
    /// <summary>
    /// Main manager for the game. Use for common shared functionality and tracking player data and progression.
    /// </summary>
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance;

        public List<CardData> rewardPool;
        public DeckData DeckData;

        public RuntimeDeckData PlayerRuntimeDeck;
        public MapInfo MapInfo;

        public CharacterData playerCharacterData;
        public EncounterData currentEncounterData;
        public int PlayerHP;
        public NodeType currentNodeType;

        [SerializeField] private bool debugMode;
        
        private void Awake()
        {
            if (Instance)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(this);
            Init();
        }

        private void Start()
        {
            SetupMap();
        }

        public void SetupMap()
        {
            StartCoroutine(IEWait((() =>
            {
                SelectCharacter(playerCharacterData);
                MapUI.current.LoadNewMap(MapInfo);
                MapUI.current.Show();
            })));
        }

        public IEnumerator IEWait(Action _onComplete)
        {
            yield return new WaitForSeconds(0.1f);
            _onComplete?.Invoke();
        }

        private void Init()
        {
            Database.Initialize();
            PlayerRuntimeDeck = new RuntimeDeckData();
            SelectCharacter(playerCharacterData);
        }

        public void SelectCharacter(CharacterData _characterData)
        {
            PlayerRuntimeDeck = new RuntimeDeckData();
            
            playerCharacterData = _characterData;

            PlayerHP = _characterData.health;
            
            foreach (var _cardData in DeckData.Cards)
            {
                PlayerRuntimeDeck.AddCard(_cardData);
            }
        }
        
        public IEnumerator GameOver()
        {
            yield break;
        }

        public IEnumerator WinBattle()
        {
            yield break;
        }
        
        private void Update()
        {
            if (debugMode)
            {
                // Example of creating a new runtime passive and adding it to passive a slot (+ also enabling the passives)
                if (Input.GetKeyDown(KeyCode.Alpha1))
                {
                    RuntimePassive passive = PassiveFactory.Create("Power Builder");
                    RuntimeCharacter player = BattleManager.current.runtimePlayer;
                    FormData form = player.GetCurrentForm();
                    player.AddPassiveToSlot(form, 0, passive);
                    player.EnablePassives(form);
                }
                
                // Example of disabling the passives
                if (Input.GetKeyDown(KeyCode.Alpha2))
                {
                    RuntimeCharacter player = BattleManager.current.runtimePlayer;
                    FormData form = player.GetCurrentForm();
                    player.DisablePassives(form);
                }
            }
        }
    }
}

