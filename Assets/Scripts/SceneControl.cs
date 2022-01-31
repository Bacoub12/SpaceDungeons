using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneControl : MonoBehaviour
{
   public void NextScene()
   {
        int indexScene = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(indexScene + 1);
   }

    public void LoadGame()
    {
        SceneManager.LoadScene(2);
    }

    public void LoadMenu()
    {
        SceneManager.LoadScene(0);
    }

    public void LoadSetting()
    {
        SceneManager.LoadScene(1);
    }

    public void Quit()
    {
        Application.Quit();
    }

}
