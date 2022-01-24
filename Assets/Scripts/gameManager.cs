using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class gameManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Physics.IgnoreLayerCollision(9, 9); //empecher les enemybullet de se frapper
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
