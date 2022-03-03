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
            Instantiate(item, transform.position, Quaternion.identity);
            Destroy(this.gameObject);
        }
    }
}
