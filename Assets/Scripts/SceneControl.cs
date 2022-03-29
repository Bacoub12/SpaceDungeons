using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneControl : MonoBehaviour
{
    private void Awake()
    {
        DontDestroyOnLoad(GameObject.Find("UI"));
        DontDestroyOnLoad(GameObject.Find("Player"));
    }

    public void NextScene()
    {
        int indexScene = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(indexScene + 1);
   }

    public void LoadGame()
    {
        SceneManager.LoadScene(2);
    }

    public void returnToLobby()
    {
        SceneManager.LoadScene(2);
        StartCoroutine(returnCoroutine());
    }

    IEnumerator returnCoroutine()
    {
        yield return new WaitForSeconds(0.5f);

        GameObject[] player_s = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject go in player_s)
        {
            if (go.name == "Player")
            {
                if (go.GetComponent<PlayerScript>().returned == false)
                {
                    Destroy(go);
                }
            }
        }
        GameObject player = GameObject.Find("Player");
        player.transform.position = new Vector3(0f, 1.7f, 4f);
        player.GetComponent<PlayerScript>().checkForUpgradeStation();

        GameObject[] things = FindObjectsOfType<GameObject>();
        foreach (GameObject go in things)
        {
            if (go.name == "UI")
            {
                if (go.GetComponent<IdentifyUI>().returned == false)
                {
                    Destroy(go);
                }
            }
        }

        UpgradeDeskScript podScript = GameObject.Find("PodAlone").GetComponent<UpgradeDeskScript>();
        podScript.uiManager = GameObject.Find("UIManager");
        podScript.upgradeDeskPanel = GameObject.Find("Canvas").transform.GetChild(3).gameObject;

        GameObject.Find("MissionCrate").GetComponent<Interactable>().onInteract.AddListener(
            GameObject.Find("SceneManager").GetComponent<SceneControl>().NextScene);

        GameObject.Find("Bed").GetComponent<Interactable>().onInteract.AddListener(
            GameObject.Find("SceneManager").GetComponent<SceneControl>().LoadMenu);

        GameObject.Find("Bed").GetComponent<Interactable>().onInteract.AddListener(
            GameObject.Find("UIManager").GetComponent<UIManager>().CursorUnlock);
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
