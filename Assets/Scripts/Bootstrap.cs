using UnityEngine;
using UnityEngine.SceneManagement;

namespace DefaultNamespace
{
    /// <summary>
    /// App entry point. Use for initializing stuff.
    /// </summary>
    public class Bootstrap : MonoBehaviour
    {
        private void Awake()
        {
            Instantiate(Resources.Load<GameManager>("Prefabs/GameManager"));
            SceneManager.LoadScene("MainMenuScene");
        }
    }
}

