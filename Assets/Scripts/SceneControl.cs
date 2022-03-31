using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneControl : MonoBehaviour
{
    public void NextScene()
    {
        int indexScene = SceneManager.GetActiveScene().buildIndex;
        if (indexScene == 2) //if on lobby (donc, about to enter game)
        {
            DontDestroyOnLoad(GameObject.Find("UI"));
            DontDestroyOnLoad(GameObject.Find("Player"));
        }
        SceneManager.LoadScene(indexScene + 1);
   }

    public void LoadGame()
    {
        if (GameObject.Find("Player"))
        {
            //if player is there, aller prendre le spawnmanager du player à la place
            Debug.Log("yo1");
            GameObject.Find("UI").transform.Find("SceneManager").gameObject.GetComponent<SceneControl>().returnToLobby();
            //returnToLobby();
        }
        else
        {
            Debug.Log("yo2");
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
        Debug.Log("yo3");
        yield return new WaitForSeconds(0.3f);

        Debug.Log("yo4");
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
            player.transform.SetPositionAndRotation(lobbySpawn.position, lobbySpawn.rotation);
        else
            player.transform.SetPositionAndRotation(tutoSpawn.position, tutoSpawn.rotation);

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
        DontDestroyOnLoad(GameObject.Find("UI"));
        DontDestroyOnLoad(GameObject.Find("Player"));
        GameObject.Find("Player").GetComponent<PlayerScript>().returned = true;
        GameObject.Find("UI").GetComponent<IdentifyUI>().returned = true;
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
