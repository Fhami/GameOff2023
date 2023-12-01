using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChanger : MonoBehaviour
{
    public void ChangeScene(string _scene)
    {
        SceneManager.LoadSceneAsync(_scene, LoadSceneMode.Single);
    }
}
