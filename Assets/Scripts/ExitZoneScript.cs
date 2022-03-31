using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ExitZoneScript : MonoBehaviour
{
    private Transform destination;
    private Transform playerTransform;
    private bool backToLobby;
    private TMP_Text floorText;
    private int floorNumber;
    private GameObject WinScreen;

    // Start is called before the first frame update
    void Start()
    {
        WinScreen = GameObject.Find("Canvas").transform.GetChild(8).gameObject;
        floorText = GameObject.Find("FloorText").GetComponent<TMP_Text>();
        playerTransform = GameObject.Find("Player").transform;
        backToLobby = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (gameObject.GetComponent<Collider>().bounds.Contains(playerTransform.position))
        {
            if (backToLobby)
            {
                /*playerTransform.gameObject.GetComponent<PlayerScript>().returned = true;
                GameObject.Find("UI").GetComponent<IdentifyUI>().returned = true;
                GameObject.Find("SceneManager").GetComponent<SceneControl>().returnToLobby();
                floorText.gameObject.SetActive(false);
                floorNumber = 0;*/
                GameObject.Find("UIManager").GetComponent<UIManager>().CursorUnlock();
                WinScreen.SetActive(true);
            }
            else
            {
                playerTransform.position = destination.position;
                floorNumber++;
                floorText.text = "Salle : " + floorNumber;
            }
        }
    }

    public void ReturnToLobby()
    {
        playerTransform.gameObject.GetComponent<PlayerScript>().returned = true;
        GameObject.Find("UI").GetComponent<IdentifyUI>().returned = true;
        GameObject.Find("SceneManager").GetComponent<SceneControl>().returnToLobby();
        floorText.gameObject.SetActive(false);
        floorNumber = 0;
    }

    public void setDestination(Transform _destination)
    {
        destination = _destination;
    }

    public void setBackToLobby(bool _backToLobby)
    {
        backToLobby = _backToLobby;
    }
}
