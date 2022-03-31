using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class enemyBulletScript : MonoBehaviour
{
    public bool poisoned;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(decay());
    }

    IEnumerator decay()
    {
        yield return new WaitForSeconds(3f);
        Destroy(gameObject);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer != 9 && collision.gameObject.layer != 7 
            && collision.gameObject.tag != "DoorDetection" && collision.gameObject.tag != "Bullet") //not enemybullet or enemy
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer != 9 && other.gameObject.layer != 7 
            && other.gameObject.tag != "DoorDetection" && other.gameObject.tag != "Bullet") //not enemybullet or enemy
        {
            Destroy(gameObject);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
