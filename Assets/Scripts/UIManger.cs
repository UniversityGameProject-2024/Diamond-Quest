using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManger : MonoBehaviour
{
    [SerializeField]
    public String firstSceneName;
    public void Play()
    {
        SceneManager.LoadScene(firstSceneName);
    }
    public void Exit()
    {
        Application.Quit();
    }
}
