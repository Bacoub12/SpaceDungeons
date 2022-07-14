using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneControl : MonoBehaviour
{
    public GameObject loadPanel;

    public void NextScene()
    {
        int indexScene = SceneManager.GetActiveScene().buildIndex;
        if (indexScene == 2) //if on lobby (donc, about to enter game)
        {
            DontDestroyOnLoad(GameObject.Find("UI"));
            DontDestroyOnLoad(GameObject.Find("Player"));
            loadPanel.SetActive(true);
            SceneManager.sceneLoaded += removeLoadingScreen;
        }
        SceneManager.LoadScene(indexScene + 1);
    }

    void removeLoadingScreen(Scene scene, LoadSceneMode mode)
    {
        loadPanel.SetActive(false);
        SceneManager.sceneLoaded -= removeLoadingScreen;
    }

    public void LoadGame()
    {
        if (GameObject.Find("Player"))
        {
            //if player is there, aller prendre le spawnmanager du player à la place
            GameObject.Find("UI").transform.Find("SceneManager").gameObject.GetComponent<SceneControl>().returnToLobby();
        }
        else
        {
            SceneManager.LoadScene(2);
        }
    }

    public void returnToLobby()
    {
        SceneManager.LoadScene(2);
        StartCoroutine(returnCoroutine());
    }

    IEnumerator returnCoroutine()
    {
        yield return new WaitForSeconds(0.3f);

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

        Transform lobbySpawn = GameObject.Find("SpawnDuLobby").transform;
        Transform tutoSpawn = GameObject.Find("SpawnDuTuto").transform;
        if (player.GetComponent<PlayerScript>().onOffSpawn)
        {
            player.transform.SetPositionAndRotation(lobbySpawn.position, lobbySpawn.rotation);
            //if spawning in lobby, destroy tuto enemies and chests
            foreach (GameObject GOinScene in FindObjectsOfType<GameObject>())
            {
                if (GOinScene.layer == 7 ||
                    GOinScene.name == "ArmorChest" || GOinScene.name == "HealthChest" || GOinScene.name == "MoneyChest")
                {
                    Destroy(GOinScene);
                }
            }
        }
        else
        {
            player.transform.SetPositionAndRotation(tutoSpawn.position, tutoSpawn.rotation);
        }

        player.GetComponent<PlayerScript>().checkForUpgradeStation();

        player.GetComponent<PlayerScript>().gunshotsMuted = false;

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


        GameObject missionCrate = GameObject.Find("MissionCrate");
        missionCrate.GetComponent<Interactable>().onInteract.AddListener(
            GameObject.Find("SceneManager").GetComponent<SceneControl>().NextScene);
        missionCrate.GetComponent<Interactable>().onInteract.AddListener(
            player.GetComponent<PlayerScript>().PlayCombatMusic);
        missionCrate.GetComponent<Interactable>().onInteract.AddListener(
            delegate {
                GameObject.Find("Canvas").transform.GetChild(7).gameObject.SetActive(true);
            });

        GameObject bed = GameObject.Find("Bed");
        bed.GetComponent<Interactable>().onInteract.AddListener(
            GameObject.Find("SceneManager").GetComponent<SceneControl>().LoadMenu);
        bed.GetComponent<Interactable>().onInteract.AddListener(
            GameObject.Find("UIManager").GetComponent<UIManager>().CursorUnlock);
        bed.GetComponent<Interactable>().onInteract.AddListener(
            player.GetComponent<PlayerScript>().StopMusic);

        GameObject key = GameObject.Find("Key");
        key.GetComponent<Interactable>().onInteract.AddListener(
            delegate {
                player.GetComponent<PlayerScript>().setKey(true);
            });
        key.GetComponent<Interactable>().onInteract.AddListener(
            delegate {
                key.SetActive(false);
            });

        if (GameObject.Find("WinPannel"))
            GameObject.Find("WinPannel").SetActive(false);

        GameObject.Find("UIManager").GetComponent<UIManager>().CursorLock();
    }

    public void LoadMenu()
    {
        if (GameObject.Find("UI") != null)
        {
            GameObject UI = GameObject.Find("UI");
            DontDestroyOnLoad(UI);
            UI.GetComponent<IdentifyUI>().returned = true;
        }
        if (GameObject.Find("Player") != null)
        {
            GameObject player = GameObject.Find("Player");
            DontDestroyOnLoad(player);
            PlayerScript playerScript = player.GetComponent<PlayerScript>();
            playerScript.gunshotsMuted = true;
            playerScript.returned = true;
        }
        SceneManager.LoadScene(0);
    }

    public void LoadSetting()
    {
        if (GameObject.Find("UI") != null)
        {
            GameObject UI = GameObject.Find("UI");
            DontDestroyOnLoad(UI);
            UI.GetComponent<IdentifyUI>().returned = true;
        }
        if (GameObject.Find("Player") != null)
        {
            GameObject player = GameObject.Find("Player");
            DontDestroyOnLoad(player);
            PlayerScript playerScript = player.GetComponent<PlayerScript>();
            playerScript.gunshotsMuted = true;
            playerScript.returned = true;
        }
        SceneManager.LoadScene(1);
    }

    public void Quit()
    {
        Application.Quit();
    }

}
