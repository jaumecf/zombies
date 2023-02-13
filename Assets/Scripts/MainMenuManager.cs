using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    public void StartGame()
    {
        SceneManager.LoadScene(2);
    }

    public void ExitGame()
    {
        Application.Quit();
        Debug.Log("Quit!");
    }

    public void LoadMultiplayerMenu()
    {
        SceneManager.LoadScene(1);
    }
}
