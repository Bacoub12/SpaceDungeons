using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerScript : MonoBehaviour
{
    int _gunId;
    [SerializeField] GameObject _bullet;
    [SerializeField] Transform _attach;
    [SerializeField] float _force = 300f;
    //[SerializeField] GameObject whatgun;

    public void OnFire()
    {
        Debug.Log("Pew PEw");
        Rigidbody rb = Instantiate(_bullet, _attach.position,_attach.rotation).GetComponent<Rigidbody>();
        rb.AddForce(_attach.forward * _force);


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
