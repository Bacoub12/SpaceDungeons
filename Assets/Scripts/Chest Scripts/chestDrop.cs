using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChestDrop : MonoBehaviour
{
    public GameObject item;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer == 0)
        {
            Vector3 spawnPos = new Vector3(transform.position.x, 0.2f, transform.position.z);
            Vector3 spawnAdjust = new Vector3(0f, 0f, 0f);

            Debug.Log(gameObject.name);
            if (gameObject.name == "ArmorDrop(Clone)")
            {
                spawnAdjust = new Vector3(0.5f, 0f, 0f);
            }
            else if (gameObject.name == "HealthDrop(Clone)")
            {
                spawnAdjust = new Vector3(0f, 0f, 0f);
            }

            GameObject spawnedItem = Instantiate(item, spawnPos + spawnAdjust, Quaternion.identity);
            spawnedItem.transform.Rotate(0.0f, 180.0f, 0.0f, Space.Self);
            Destroy(this.gameObject);
        }
    }
}
