using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class gameManager : MonoBehaviour
{
    public UIManager UIManager;

    void Start()
    {
        Physics.IgnoreLayerCollision(9, 9); //enemybullet entre eux
        Physics.IgnoreLayerCollision(6, 11); //joueur et visuals
        Physics.IgnoreLayerCollision(8, 11); //joueur (layer gun) et visuals
        Physics.IgnoreLayerCollision(7, 11); //ennemis et visuals
        Physics.IgnoreLayerCollision(9, 11); //enemybullet et visuals
        Physics.IgnoreLayerCollision(9, 7); //enemybullet et ennemis
        Physics.IgnoreLayerCollision(10, 11); //interactable (donc les chests) et drops
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
