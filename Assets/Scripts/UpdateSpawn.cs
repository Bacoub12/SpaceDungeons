using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpdateSpawn : MonoBehaviour
{
    bool onOff = false;

    public bool getOnOff()
    {
        return onOff;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name == "Player")
        {
            onOff = true;
            other.gameObject.GetComponent<PlayerScript>().onOffSpawn = true;
        }
    }
}
