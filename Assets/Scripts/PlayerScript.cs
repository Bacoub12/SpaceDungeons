using StarterAssets;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using Random = UnityEngine.Random;

public class PlayerScript : MonoBehaviour
{
    int gunId = 0;
    int canShootShotgun = 0;
    int canShootRifle = 0;
    public int money = 0;
    [SerializeField] GameObject _bullet;
    [SerializeField] GameObject UIManager;
    [SerializeField] Transform _attach;
    [SerializeField] private Camera camera;
    [SerializeField] float _force = 1000f;
    [SerializeField] private InputActionAsset _actionAsset = default;
    [SerializeField] private UI_Inventory uiInventory;

    bool autoStop = false;
    bool interaction = false;
    bool onOffCrouch = false;
    bool pause = false;
    float timer = 0.0f;
    FirstPersonController firstPersonController;
    CapsuleCollider capsuleCollider;
    CharacterController _CharacterController;
    Rigidbody rb;
    public LayerMask interactableLayerMask = 10;
    private Interactable interactable;
    private Inventory inventory;
    private Item item;
    private MoneyScript moneyScript;

    //[SerializeField] GameObject whatgun;

    private void Start()
    {
        _CharacterController = GetComponent<CharacterController>();
        capsuleCollider = GetComponentInChildren<CapsuleCollider>();
        firstPersonController = GetComponent<FirstPersonController>();
        inventory = GameObject.Find("InventoryManager").GetComponent<Inventory>();
        item = GetComponent<Item>();

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
            switch (gunId)
            {
                case 0: //pistol
                    rb = Instantiate(_bullet, _attach.position, _attach.rotation).GetComponent<Rigidbody>();
                    rb.AddForce(_attach.forward * _force);
                    break;

                case 1: // shotgun
                    if (canShootShotgun == 0)
                        StartCoroutine(PumpShotgun());
                    break;

                case 2: // rifle
                    autoStop = true;
                    if (canShootRifle == 0)
                        StartCoroutine(AutomaticRifle());
                    break;
            }
        }
    }

    IEnumerator AutomaticRifle()
    {
        while (autoStop)
        {
            canShootRifle = 1;
            rb = Instantiate(_bullet, _attach.position, _attach.rotation).GetComponent<Rigidbody>();
            rb.AddForce(_attach.forward * _force);
            yield return new WaitForSeconds(0.2f);
            canShootRifle = 0;
        }
    }

    IEnumerator PumpShotgun()
    {
        canShootShotgun = 1;
        for (int i = 0; i < 40; i++)
        {
            float randomX = Random.Range(-20f, 20f);
            float randomY = Random.Range(-20f, 20f);
            float randomZ = Random.Range(-20f, 20f);
            Rigidbody rb = Instantiate(_bullet, _attach.position, _attach.rotation).GetComponent<Rigidbody>();
            rb.transform.Rotate(randomX, randomY, randomZ);
            rb.AddForce(rb.transform.forward * _force);
        }
        yield return new WaitForSeconds(1.0f);
        canShootShotgun = 0;
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
        if (other.gameObject.layer == 14)
        {
            if(other.gameObject.tag == "Money")
            {
                moneyScript = other.gameObject.GetComponent<MoneyScript>();
                money += moneyScript.getMoneyValue();
                Destroy(other.gameObject);
                Debug.Log("Money : " + money);
            }
            else if (other.gameObject.tag == "Helmet")
            {
                item = other.gameObject.GetComponent<Item>(); //chercher le script
                inventory.AddItem(item);
                Destroy(other.gameObject);
                foreach (Item i in inventory.GetItemList())
                {
                    Debug.Log("List : " + i.getType());
                    
                }
            }
            else if (other.gameObject.tag == "Chest")
            {
                item = other.gameObject.GetComponent<Item>(); //chercher le script
                inventory.AddItem(item);
                Destroy(other.gameObject);
                foreach (Item i in inventory.GetItemList())
                {
                    Debug.Log("List : " + i.getType());
                }
            }
            else if (other.gameObject.tag == "Pantalon")
            {
                item = other.gameObject.GetComponent<Item>(); //chercher le script
                inventory.AddItem(item);
                Destroy(other.gameObject);
                foreach (Item i in inventory.GetItemList())
                {
                    Debug.Log("List : " + i.getType());
                }
            }
            else if (other.gameObject.tag == "Boot")
            {
                item = other.gameObject.GetComponent<Item>(); //chercher le script
                inventory.AddItem(item);
                Destroy(other.gameObject);
                foreach (Item i in inventory.GetItemList())
                {
                    Debug.Log("List : " + i.getType());
                }
            }
            else if (other.gameObject.tag == "Health")
            {
                item = other.gameObject.GetComponent<Item>(); //chercher le script
                inventory.AddItem(item);
                Destroy(other.gameObject);
                foreach (Item i in inventory.GetItemList())
                {
                    Debug.Log("List : " + i.getType());
                }
            }
            //Debug.Log("layer de  : " + other.gameObject.layer);
            //Destroy(other.gameObject);
        }
        else
        {
            //Debug.Log("layer autre que collectible  : " + other.gameObject.layer);
        }
    }

    void Update()
    {
        RaycastHit hit;
        if (Physics.Raycast(camera.transform.position, camera.transform.forward, out hit, 2, interactableLayerMask))
        {
            if (hit.collider.GetComponent<Interactable>() != false)
            {
                string objectName = hit.collider.name;
                switch (objectName)
                {
                    case "Chest":
                        if (hit.collider.gameObject.GetComponent<ChestScript>().isOpened() == false)
                            UIManager.GetComponent<UIManager>().Interactive(true, "Appuyez sur F pour ouvrir le coffre");
                        else
                            UIManager.GetComponent<UIManager>().Interactive(false, "");
                        break;
                    case "Door":
                        UIManager.GetComponent<UIManager>().Interactive(true, "Appuyez sur F pour ouvrir la porte");
                        break;
                    case "Key":
                        UIManager.GetComponent<UIManager>().Interactive(true, "Appuyez sur F pour prendre la clé");
                        break;
                    case "Teleporter":
                        UIManager.GetComponent<UIManager>().Interactive(true, "Appuyez sur F pour utiliser le téléporteur");
                        break;
                    default:
                        UIManager.GetComponent<UIManager>().Interactive(true, "Appuyez sur F pour intéragir");
                        break;
                }

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
            UIManager.GetComponent<UIManager>().Interactive(false, "");
        }
        
    }
    public void OnInteract() // le boutons
    {
        if(interactable == true)
        {
            interactable.onInteract.Invoke();
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
