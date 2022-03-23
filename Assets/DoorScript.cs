using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorScript : MonoBehaviour
{
    private GameObject leftDoor;
    private GameObject rightDoor;
    private Animator DoorController;
    private bool doorOpen = false;
    // Start is called before the first frame update
    void Start()
    {
        leftDoor = transform.GetChild(0).gameObject;
        rightDoor = transform.GetChild(1).gameObject;
        DoorController = GetComponent<Animator>();
    }

    private void OnTriggerEnter(Collider other)
    {
        Physics.IgnoreLayerCollision(1, 7);
        Physics.IgnoreLayerCollision(9, 14);

        if(other.gameObject.layer == 8)
        {
            doorOpen = true;
            DoorController.Play("OpenDoor");
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

}
