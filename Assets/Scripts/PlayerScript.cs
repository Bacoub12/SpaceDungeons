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
    [SerializeField] float _force = 100f;

    public LayerMask interactableLayerMask = 10;
    [SerializeField] private Camera camera;
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
        
    }

    void Update() ////// faire que le hit revienne après avoir regarder ailleur, même s'il est pas venu en contact avec d'autre object
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
