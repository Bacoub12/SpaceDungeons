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
using UnityEngine.UI;

public class PlayerScript : MonoBehaviour
{
    int gunId = 0;
    int canShootShotgun = 0;
    int canShootRifle = 0;
    public int health = 100;
    public int armure = 100;
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
    private UpgradeDeskScript upgradeDeskScript;

    private float pistolDamage, shotgunDamage, rifleDamage;
    private bool damageUpgrade1, damageUpgrade2, damageUpgrade3;

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

        if (GameObject.Find("UpgradeDesk") != null)
            upgradeDeskScript = GameObject.Find("UpgradeDesk").GetComponent<UpgradeDeskScript>();

        pistolDamage = 10f;
        shotgunDamage = 2f; //? vu que y'a plus de balles
        rifleDamage = 5f; //même raisonnement
        damageUpgrade1 = false;
        damageUpgrade2 = false;
        damageUpgrade3 = false;
    }


    private void FireAction_canceled(InputAction.CallbackContext obj)
    {
        autoStop = false;
        StopCoroutine(AutomaticRifle());
    }

    private void FireAction_performed(InputAction.CallbackContext obj)
    {
        if (upgradeDeskScript != null)
        {
            if (!UIManager.GetComponent<UIManager>().getPause() && !upgradeDeskScript.checkIfActive())
            {
                shoot();
            }
        }
        else if (!UIManager.GetComponent<UIManager>().getPause())
        {
            shoot();
        }
    }

    private void shoot()
    {
        switch (gunId)
        {
            case 0: //pistol
                GameObject bullet = Instantiate(_bullet, _attach.position, _attach.rotation);
                bullet.GetComponent<BulletScript>().setDamageParams(pistolDamage, damageUpgrade1, damageUpgrade2, damageUpgrade3);
                rb = bullet.GetComponent<Rigidbody>();
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

    IEnumerator AutomaticRifle()
    {
        while (autoStop)
        {
            canShootRifle = 1;
            GameObject bullet = Instantiate(_bullet, _attach.position, _attach.rotation);
            bullet.GetComponent<BulletScript>().setDamageParams(rifleDamage, damageUpgrade1, damageUpgrade2, damageUpgrade3);
            rb = bullet.GetComponent<Rigidbody>();
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
            GameObject bullet = Instantiate(_bullet, _attach.position, _attach.rotation);
            bullet.GetComponent<BulletScript>().setDamageParams(shotgunDamage, damageUpgrade1, damageUpgrade2, damageUpgrade3);
            rb = bullet.GetComponent<Rigidbody>();
            rb.transform.Rotate(randomX, randomY, randomZ);
            rb.AddForce(rb.transform.forward * _force);
        }
        yield return new WaitForSeconds(1.0f);
        canShootShotgun = 0;
    }

    public void setDamageUpgrades(bool _damageUpgrade1, bool _damageUpgrade2, bool _damageUpgrade3)
    {
        damageUpgrade1 = _damageUpgrade1;
        damageUpgrade2 = _damageUpgrade2;
        damageUpgrade3 = _damageUpgrade3;
        //Debug.Log("damage upgrades updated: " + damageUpgrade1 + " " + damageUpgrade2 + " " + damageUpgrade3);
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
            if (other.gameObject.tag == "Money")
            {
                moneyScript = other.gameObject.GetComponent<MoneyScript>();
                money += moneyScript.getMoneyValue();
                Destroy(other.gameObject);
                Debug.Log("Money : " + money);
            }
            else if (other.gameObject.tag == "Helmet" || other.gameObject.tag == "Chestplate" || other.gameObject.tag == "Gloves" || other.gameObject.tag == "Boot" || other.gameObject.tag == "Health")
            {
                item = other.gameObject.GetComponent<Item>(); //chercher le script
                //Debug.Log("item" + item.getType());
                inventory.AddItem(item);

                GameObject itemObject = UIManager.GetComponent<UIManager>().AjoutItemUi(item);
                GameObject buttonDrop = itemObject.transform.GetChild(1).gameObject;
                GameObject inventoryManager = GameObject.Find("InventoryManager");
                Image theImage = itemObject.transform.GetChild(0).gameObject.GetComponent<Image>();
                buttonDrop.GetComponent<Button>().onClick.AddListener(delegate { inventoryManager.GetComponent<Inventory>().Drop(theImage); });

                Destroy(other.gameObject);
            }
            else if (other.gameObject.layer == 9)
            {
                //mettre des if pour le nombre de degat recu
                Damage();
            }
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
                    case "ArmorChest": case "HealthChest": case "MoneyChest":
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

    private void Damage()
    {
        // mettre que les degat rentre dans larmnure et aprse le restant des degat rentre dans la vie
        if (armure > 0)
        {
            if (health > 0)
            {
                health -= 10;
            }
            else
            {
                UIManager.GetComponent<UIManager>().DeathScreen(true);
            }
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
            Debug.Log("/Vegu");
        }
    }

    public void OnPause()
    {
        //si l'ecran update est là, l'enlever
        if (upgradeDeskScript != null)
        {
            if (upgradeDeskScript.checkIfActive() == true)
                upgradeDeskScript.toggleUpgradeInterface();
        }
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
