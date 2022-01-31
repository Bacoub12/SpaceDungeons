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
    [SerializeField] GameObject UIManager;
    [SerializeField] Transform _attach;
    [SerializeField] float _force = 100f;
    private bool pause = false;

    public LayerMask interactableLayerMask = 10;
    [SerializeField] private Camera camera;
    private Interactable interactable;

    //[SerializeField] GameObject whatgun;

    public void OnFire()
    {
        if (!pause)
        {
            //Debug.Log("Pew PEw");
            Rigidbody rb = Instantiate(_bullet, _attach.position, _attach.rotation).GetComponent<Rigidbody>();
            rb.AddForce(_attach.forward * _force);
        }


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

    void Update()
    {
        RaycastHit hit;
        if (Physics.Raycast(camera.transform.position, camera.transform.forward, out hit, 2, interactableLayerMask))
        {
            if (hit.collider.GetComponent<Interactable>() != false)
            {
                _interact.SetActive(true);

                if (interactable == null || interactable.ID != hit.collider.GetComponent<Interactable>().ID)
                {
                    interactable = hit.collider.GetComponent<Interactable>();
                    Debug.Log("new interactable " + interactable);
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

    public void OnPause()
    {
        UIManager.GetComponent<UIManager>().PauseGame();
    }
}
