using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class gameManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Physics.IgnoreLayerCollision(9, 9); //empecher les enemybullet de se frapper entre eux
        Physics.IgnoreLayerCollision(6, 11); //empecher le joueur et les visuals de se frapper
        Physics.IgnoreLayerCollision(7, 11); //empecher les ennemis et les visuals de se frapper
        Physics.IgnoreLayerCollision(9, 11); //empecher les enemybullet et les visuals de se frapper
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
