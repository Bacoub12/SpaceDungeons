using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChestDrop : MonoBehaviour
{
    public GameObject item;
    public float chestHeight;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (transform.position.y < -100f)
        {
            Vector3 spawnPos = new Vector3(transform.position.x, chestHeight + 0.2f, transform.position.z);
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
            Vector3 spawnPos = new Vector3(transform.position.x, chestHeight + 0.2f, transform.position.z);
            Vector3 spawnAdjust = new Vector3(0f, 0f, 0f); //modifiable au cas ou

            //Debug.Log(gameObject.name);
            /*
            if (gameObject.name == "ArmorDrop(Clone)")
            {
                spawnAdjust = new Vector3(0f, 0f, 0f);
            }
            else if (gameObject.name == "HealthDrop(Clone)")
            {
                spawnAdjust = new Vector3(0f, 0f, 0f);
            }
            else if (gameObject.name == "MoneyDrop(Clone)")
            {
                spawnAdjust = new Vector3(0f, 0f, 0f);
            }
            */

            GameObject spawnedItem = Instantiate(item, spawnPos + spawnAdjust, Quaternion.identity);
            spawnedItem.transform.Rotate(0.0f, 180.0f, 0.0f, Space.Self);
            if (gameObject.name == "MoneyDrop(Clone)")
                spawnedItem.transform.Rotate(90.0f, 0.0f, 0.0f, Space.Self);
            Destroy(this.gameObject);
        }
    }
}
