using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bulletScript : MonoBehaviour
{
    float timeBeforeDestroy = 3f;

    void Start()
    {
        Destroy(gameObject, timeBeforeDestroy);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag != "Player")
        {
            Debug.Log("tag : " + other.gameObject.tag);
            Destroy(gameObject);
        }
        else
        {
            Debug.Log("tag : " + other.gameObject.tag);
            Destroy(gameObject);
        }
    }
}
