using StarterAssets;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using Random = UnityEngine.Random;

public class PlayerScript : MonoBehaviour
{
    int gunId = 0;
    [SerializeField] GameObject _bullet;
    [SerializeField] GameObject UIManager;
    [SerializeField] GameObject ShotgunScript;
    [SerializeField] Transform _attach;
    [SerializeField] private Camera camera;
    [SerializeField] float _force = 500f;
    [SerializeField] private InputActionAsset _actionAsset = default;
    FirstPersonController firstPersonController;

    

    bool autoStop = false;
    bool interaction = false;
    bool onOffCrouch = false;
    bool pause = false;
    float timer = 0.0f;
    CapsuleCollider capsuleCollider;
    CharacterController _CharacterController;
    Rigidbody rb;
    public LayerMask interactableLayerMask = 10;
    private Interactable interactable;

    //[SerializeField] GameObject whatgun;

    private void Start()
    {
        _CharacterController = GetComponent<CharacterController>();
        capsuleCollider = GetComponentInChildren<CapsuleCollider>();
        firstPersonController = GetComponent<FirstPersonController>();

        var fireAction = _actionAsset.FindAction("Fire");
        fireAction.performed += FireAction_performed;
        fireAction.canceled += FireAction_canceled;
        fireAction.Enable();

        
    }

    private void FireAction_canceled(InputAction.CallbackContext obj)
    {
        autoStop = false;
        StopCoroutine(AutomaticRifle());
    }

    private void FireAction_performed(InputAction.CallbackContext obj)
    {
        if (!UIManager.GetComponent<UIManager>().getPause())
        {
            autoStop = false;
            switch (gunId)
            {
                case 0: //pistol
                    autoStop = false;
                    rb = Instantiate(_bullet, _attach.position, _attach.rotation).GetComponent<Rigidbody>();
                    rb.AddForce(_attach.forward * _force);
                    break;

                case 1: // shotgun
                    autoStop = false;
                    //ShotgunScript.GetComponent<ShotgunScript>().Shotgun();
                    break;

                case 2: // rifle
                    autoStop = true;
                    StartCoroutine(AutomaticRifle());
                    break;
            }
        }
        
    }

    IEnumerator AutomaticRifle()
    {
        while (autoStop)
        {
            rb = Instantiate(_bullet, _attach.position, _attach.rotation).GetComponent<Rigidbody>();
            rb.AddForce(_attach.forward * _force);
            yield return new WaitForSeconds(0.1f);
        }
    }

    private void OnGun1()
    {
        gunId = 0;
    }

    private void OnGun2()
    {
        gunId = 1;
    }
    private void OnGun3()
    {
        gunId = 2;
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
                    interaction = true;
                    interactable = hit.collider.GetComponent<Interactable>();
                    Debug.Log("new interactable " + interactable);
                }
                else
                {
                    interaction = false;
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
        if(interactable == true)
        {
            interactable.onInteract.Invoke();// peut caus� des crashs, mais incapable de reproduire...
        }
        else
        {
            Debug.Log("/Veg");
        }
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
