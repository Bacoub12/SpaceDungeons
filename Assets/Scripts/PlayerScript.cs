using StarterAssets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class PlayerScript : MonoBehaviour
{
    int gunId = 0;
    [SerializeField] GameObject _bullet;
    [SerializeField] GameObject UIManager;
    //[SerializeField] GameObject FirstPersonController;
    [SerializeField] Transform _attach;
    [SerializeField] private Camera camera;
    [SerializeField] float _force = 300f;
    FirstPersonController firstPersonController;

    bool interaction = false;
    private float timer = 0.0f;
    bool onOffCrouch = false;
    private bool pause = false;
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
    }

    public void OnFire()
    {
        if (!UIManager.GetComponent<UIManager>().getPause())
        {
            switch (gunId)
            {
                case 0: //pistol
                    rb = Instantiate(_bullet, _attach.position, _attach.rotation).GetComponent<Rigidbody>();
                    rb.AddForce(_attach.forward * _force);
                    break;

                case 1: // rifle
                    AutoFunction();
                    break;
            }
        }
    }

    private void AutoFunction()
    {
        AutomaticRifle();
    }

    IEnumerable AutomaticRifle()
    {   
        rb = Instantiate(_bullet, _attach.position, _attach.rotation).GetComponent<Rigidbody>();
        rb.AddForce(_attach.forward * _force);
        yield return new WaitForSeconds(0.05f);
    }

    private void OnGun1()
    {
        gunId = 0;
    }

    private void OnGun2()
    {
        gunId = 1;
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
            interactable.onInteract.Invoke();// peut causé des crashs, mais incapable de reproduire...
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
