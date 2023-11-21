using System.Collections;
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

        [SerializeField] private bool debugMode;
        
        private void Awake()
        {
            Instance = this;
            DontDestroyOnLoad(this);
            Init();
        }

        private void Init()
        {
            Database.Initialize();
            PlayerRuntimeDeck = new RuntimeDeckData();
        }

        public IEnumerator GameOver()
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

