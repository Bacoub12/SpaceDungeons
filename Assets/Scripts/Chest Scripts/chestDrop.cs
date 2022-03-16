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
        if (transform.position.y < -50f)
        {
            Vector3 spawnPos = new Vector3(transform.position.x, 0.2f, transform.position.z);
            Vector3 spawnAdjust = new Vector3(0f, 0f, 0f); //modifiable au cas ou
            GameObject spawnedItem = Instantiate(item, spawnPos + spawnAdjust, Quaternion.identity);
            spawnedItem.transform.Rotate(0.0f, 180.0f, 0.0f, Space.Self);
            if (gameObject.name == "MoneyDrop(Clone)")
                spawnedItem.transform.Rotate(90.0f, 0.0f, 0.0f, Space.Self);
            Destroy(this.gameObject);
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer == 0)
        {
            Vector3 spawnPos = new Vector3(transform.position.x, transform.position.y, transform.position.z);
            Vector3 spawnAdjust = new Vector3(0f, 0f, 0f); //modifiable au cas ou

            //Debug.Log(gameObject.name);
            
            if (gameObject.name == "HelmetDrop(Clone)")
            {
                spawnAdjust = new Vector3(0f, 0.25f, 0f);
            } 
            else if (gameObject.name == "GlovesDrop(Clone)")
            {
                spawnAdjust = new Vector3(0f, 0.2f, 0f);
            }
            else if (gameObject.name == "MoneyDrop(Clone)")
            {
                spawnAdjust = new Vector3(0f, 0.2f, 0f);
            }

            GameObject spawnedItem = Instantiate(item, spawnPos + spawnAdjust, Quaternion.identity);
            spawnedItem.transform.Rotate(0.0f, 180.0f, 0.0f, Space.Self);
            if (gameObject.name == "MoneyDrop(Clone)")
                spawnedItem.transform.Rotate(90.0f, 0.0f, 0.0f, Space.Self);
            if (gameObject.name == "BootsDrop(Clone)")
                spawnedItem.transform.Rotate(-90.0f, 0.0f, 0.0f, Space.Self);
            Destroy(this.gameObject);
        }
    }
}
