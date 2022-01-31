using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bulletScript : MonoBehaviour
{
    float timeBeforeDestroy = 3f;
    [SerializeField] private ParticleSystem particules;

    void Start()
    {
        Destroy(gameObject, timeBeforeDestroy);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag != "Player")
        {
            //Debug.Log("tag : " + other.gameObject.tag);
            Instantiate(particules, gameObject.transform.position, Quaternion.identity);
            Destroy(gameObject);

        }
        else
        {
            //Debug.Log("tag else : " + other.gameObject.tag);
            //Destroy(gameObject);
        }
    }
}
