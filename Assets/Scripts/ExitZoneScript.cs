using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExitZoneScript : MonoBehaviour
{
    private Transform destination;
    private Transform playerTransform;
    private bool backToLobby;

    // Start is called before the first frame update
    void Start()
    {
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
                playerTransform.gameObject.GetComponent<PlayerScript>().returned = true;
                GameObject.Find("SceneManager").GetComponent<SceneControl>().returnToLobby();
            }
            else
            {
                playerTransform.position = destination.position;
            }
        }
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
