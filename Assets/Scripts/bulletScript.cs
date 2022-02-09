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
        if (other.gameObject.tag != "Player" && other.gameObject.tag != "Gun" && other.gameObject.tag != "Bullet")
        {
            Debug.Log("tag de  : " + other.gameObject.tag);
            Instantiate(particules, gameObject.transform.position, Quaternion.identity);
            Destroy(gameObject);

        }
        else
        {
            Debug.Log("tag else : " + other.gameObject.tag);
            //Destroy(gameObject);
        }
    }
}
