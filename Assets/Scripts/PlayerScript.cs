using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class PlayerScript : MonoBehaviour
{
    int _gunId;
    [SerializeField] GameObject _interact;
    [SerializeField] GameObject _bullet;
    [SerializeField] Transform _attach;
    [SerializeField] float _force = 300f;

    public LayerMask interactableLayerMask = 10;
    [SerializeField] private Camera camera;
    //private Interactable onInteract;
    private Interactable interactable;

    //[SerializeField] GameObject whatgun;

    public void OnFire()
    {
        //Debug.Log("Pew PEw");
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

    private void OnTriggerEnter(Collider other)
    {
        /*//Debug.Log("tset");
        if (other.gameObject.layer == 10)
        {
            //Debug.Log("Collectable detecter");
            _interact.SetActive(true);
            
        }
        else
        {
            _interact.SetActive(false);
            //Debug.Log("Layer + " + other.gameObject.layer);
        }*/
    }

    void Update()
    {
        RaycastHit hit;
        if (Physics.Raycast(camera.transform.position, camera.transform.forward, out hit, 2, interactableLayerMask))
        {
            if (hit.collider.GetComponent<Interactable>() != false)
            {
                if (interactable == null || interactable.ID != hit.collider.GetComponent<Interactable>().ID)
                {
                    _interact.SetActive(true);
                    interactable = hit.collider.GetComponent<Interactable>();
                    Debug.Log("new interactable");
                }
            }
        }
        else
        {
            _interact.SetActive(false);
        }
    }
    public void OnInteract() // le boutons
    {
        interactable.onInteract.Invoke();
    }
}
