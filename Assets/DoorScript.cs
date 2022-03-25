using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorScript : MonoBehaviour
{
    private GameObject LeLock;
    private Animator DoorController;
    private bool key;
    private bool doorOpen = false;

    // Start is called before the first frame update
    void Start()
    {
        DoorController = GetComponent<Animator>();
        try
        {
            LeLock = transform.GetChild(3).gameObject;
        }
        catch (UnityException ex)
        {
            Debug.Log(ex);
            LeLock = null;
        }
        
    }


    private void OnTriggerEnter(Collider other)
    {
        Physics.IgnoreLayerCollision(1, 7);
        Physics.IgnoreLayerCollision(9, 14);
        key = GameObject.Find("Player").GetComponent<PlayerScript>().getKey();
        if(other.gameObject.layer == 8)
        {
            if (key == true && LeLock == null)
            {
                doorOpen = true;
                DoorController.Play("OpenDoor");
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        Physics.IgnoreLayerCollision(1, 7);
        Physics.IgnoreLayerCollision(9, 14);
        if (other.gameObject.layer == 8)
        {
            if (doorOpen)
            {
                doorOpen = false;
                DoorController.Play("CloseDoor");
            }
        }
    }

    public void OpenTheDoor()
    {
        LeLock = null;
        try
        {
            LeLock = transform.GetChild(3).gameObject;
            GameObject.Find("Player").GetComponent<PlayerScript>().setLeLock(true);
        }
        catch (UnityException ex)
        {
            LeLock = null;
        }
        if(LeLock == null)
        {
            DoorController.Play("OpenDoor");
            GameObject.Find("Player").GetComponent<PlayerScript>().setLeLock(false);
            doorOpen = true;
            GameObject.Find("Player").GetComponent<PlayerScript>().setOnce(true);
        }
        //Debug.Log("LeLock : " + LeLock);
    }
}
