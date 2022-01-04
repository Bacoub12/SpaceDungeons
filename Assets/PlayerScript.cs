using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerScript : MonoBehaviour
{
    int gunId;
    [SerializeField] GameObject bullet;
    //[SerializeField] GameObject whatgun;

    public void OnFire()
    {
        Debug.Log("Pew PEw");
        Instantiate(bullet, transform.position, transform.rotation);
        /*switch (gunId)
        {
            case 0: //pistol
            Instantiate
                break;

            case 1: // rifle
                break;
        }*/
    }
    
    
}
