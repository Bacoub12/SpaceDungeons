using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PodScript : MonoBehaviour
{
    private Animator ArmorPodController;
    private bool doorOpen = false;
    
    void Start()
    {
        ArmorPodController = GetComponent<Animator>();
    }

    private void OnTriggerEnter(Collider other)
    {
        Physics.IgnoreLayerCollision(1, 7);
        Physics.IgnoreLayerCollision(9, 14);
        if (other.gameObject.layer == 8)
        {
            doorOpen = true;
            ArmorPodController.Play("OpenDoorPod");
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
                ArmorPodController.Play("CloseDoorPod");
            }
        }
    }
}
