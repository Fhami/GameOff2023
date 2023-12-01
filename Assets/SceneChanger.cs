using System.Collections;
using System.Collections.Generic;
using DefaultNamespace;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChanger : MonoBehaviour
{
    public void ChangeScene(string _scene)
    {
        SceneManager.LoadSceneAsync(_scene, LoadSceneMode.Single);
    }

    public void ResetMap()
    {
        if (GameManager.Instance)
        {
            GameManager.Instance.SetupMap();
        }
    }
}
