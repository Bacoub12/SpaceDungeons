using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomTerrain : MonoBehaviour
{
    // Start is called before the first frame update

    public GameObject[] Layout;
    void Start()
    {
        int x = 0;
        var rndNombre = 0;
        var ancientNombre = 0;
        var positionX = 0;

        for (x = 0; x<10; x++){ 

            while(rndNombre == ancientNombre)
            {
                rndNombre = Random.Range(1, 16);
            }

            ancientNombre = rndNombre;

            Instantiate(Layout[rndNombre-1], new Vector3(positionX, 0, 0), Quaternion.identity);

            positionX += 200;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
