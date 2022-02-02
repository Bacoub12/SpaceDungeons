using StarterAssets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class PlayerScript : MonoBehaviour
{
    int _gunId;
    [SerializeField] GameObject _bullet;
    [SerializeField] GameObject UIManager;
    //[SerializeField] GameObject FirstPersonController;
    [SerializeField] Transform _attach;
    [SerializeField] private Camera camera;
    [SerializeField] float _force = 100f;
    FirstPersonController firstPersonController;

    private float timer = 0.0f;
    bool onOffCrouch = false;
    private bool pause = false;
    CapsuleCollider capsuleCollider;
    CharacterController _CharacterController;
    public LayerMask interactableLayerMask = 10;
    private Interactable interactable;

    //[SerializeField] GameObject whatgun;

    private void Start()
    {
        _CharacterController = GetComponent<CharacterController>();
        capsuleCollider = GetComponentInChildren<CapsuleCollider>();
        firstPersonController = GetComponent<FirstPersonController>();
    }

    public void OnFire()
    {
        if (!UIManager.GetComponent<UIManager>().getPause())
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
                UIManager.GetComponent<UIManager>().Interactive(true);
                if (interactable == null || interactable.ID != hit.collider.GetComponent<Interactable>().ID)
                {
                    interactable = hit.collider.GetComponent<Interactable>();
                    Debug.Log("new interactable " + interactable);
                }
            }
        }
        else
        {
            UIManager.GetComponent<UIManager>().Interactive(false);
        }
    }
    public void OnInteract() // le boutons
    {
        interactable.onInteract.Invoke();  // peut causé des crashs, mais incapable de reproduire...
    }

    public void OnPause()
    {
        UIManager.GetComponent<UIManager>().PauseGame();
    }

    public void OnCrouch()
    {
        Debug.Log(onOffCrouch);
        if (!onOffCrouch)
        {
            _CharacterController.height = 1.0f;
            capsuleCollider.height = 1.0f;
            firstPersonController.GroundedOffset = -1.0f;
            onOffCrouch = true;
        }
        else
        {
            StartCoroutine(Start2());
            firstPersonController.GroundedOffset = -0.14f;
            onOffCrouch = false;
        }


    }
    IEnumerator Start2()
    {
        for (float f = 1; f <= 2; f += 0.02f)
        {
            _CharacterController.height = f;
            capsuleCollider.height = f;
            if(f >= 1.999)
            {
                _CharacterController.height = 2f;
                capsuleCollider.height = 2f;
            }
            yield return new WaitForSeconds(0.001f);
        }
       
    }
}
