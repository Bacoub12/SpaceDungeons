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
using TMPro;

public class PlayerScript : MonoBehaviour
{
    int gunId = 0;
    int canShootPistol = 0;
    int canShootShotgun = 0;
    int canShootRifle = 0;
    int baseHealth = 100;
    int baseArmure = 100;
    public int health, maxHealth; //maxhealth pourra être utilisé pour empêcher le joueur de 'overheal'
    public int armure, maxArmure; //pareil pour maxarmure
    public int money = 0;
    public bool returned = false;
    [SerializeField] GameObject _bullet;
    [SerializeField] GameObject UIManager;
    [SerializeField] GameObject UpdateSpawn;
    [SerializeField] Transform _attach;
    [SerializeField] private Camera camera;
    [SerializeField] float _force = 1000f;
    [SerializeField] private InputActionAsset _actionAsset = default;
    [SerializeField] private TMP_Text _moneyText;
    [SerializeField] private TMP_Text _healthText;
    [SerializeField] private TMP_Text _armorText;
    [SerializeField] private GameObject gunAnim;
    [SerializeField] private GameObject ThePlayer;
    [SerializeField] private Transform TutoSpawn;
    [SerializeField] private Transform LobbySpawn;
    [SerializeField] private AudioClip PistolSound;
    [SerializeField] private AudioSource ShotgunSound;
    [SerializeField] private AudioClip LMGSound;
    [SerializeField] private AudioSource GruntSound1;
    [SerializeField] private AudioSource GruntSound2;
    [SerializeField] private AudioSource GruntSound3;
    [SerializeField] private AudioSource HPRegenSound;
    [SerializeField] private AudioSource ShieldCrackSound;
    [SerializeField] private AudioSource ShieldRegenSound;
    [SerializeField] private AudioClip MoneyPickUpSound;
    [SerializeField] private AudioSource MoneyPickUpSoundSource;
    [SerializeField] private AudioSource TutorialSound;
    [SerializeField] private AudioSource CombatSound;
    [SerializeField] private AudioSource LobbySound;
    [SerializeField] private AudioSource DeathSound;
    [SerializeField] private GameObject crossHair;
    [SerializeField] private AudioClip HitmarkSound;

    AudioSource hitmark;
    AudioSource audioSourcePistol;
    AudioSource audioSourceLMG;
    AudioSource audioSourceMoney;

    bool autoStop = false;
    bool interaction = false;
    bool onOffCrouch = false;
    bool pause = false;
    bool key = false;
    bool LeLock = true;
    bool once = false;
    bool onceAgain = false;
    bool waterRun = false;
    public bool gunshotsMuted = false;
    public bool onOffSpawn;
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
    private FlingPlayerScript flingPlayerScript;

    private float pistolDamage, shotgunDamage, rifleDamage;
    private bool damageUpgrade1, damageUpgrade2, damageUpgrade3;
    private bool healthUpgrade1, healthUpgrade2, healthUpgrade3;
    private bool armureUpgrade1, armureUpgrade2, armureUpgrade3;
    private bool poisoned, dead;
    private bool recentlyFlung;
    private bool switching;

    //[SerializeField] GameObject whatgun;

    private void Start()
    {
        hitmark = GetComponent<AudioSource>();
        audioSourcePistol = GetComponent<AudioSource>();
        audioSourceLMG = GetComponent<AudioSource>();
        audioSourceMoney = GetComponent<AudioSource>();
        _CharacterController = GetComponent<CharacterController>();
        capsuleCollider = GetComponentInChildren<CapsuleCollider>();
        firstPersonController = GetComponent<FirstPersonController>();
        inventory = GameObject.Find("InventoryManager").GetComponent<Inventory>();
        item = GetComponent<Item>();

        var fireAction = _actionAsset.FindAction("Fire");
        fireAction.performed += FireAction_performed;
        fireAction.canceled += FireAction_canceled;
        fireAction.Enable();

        checkForUpgradeStation();

        flingPlayerScript = gameObject.GetComponent<FlingPlayerScript>();

        pistolDamage = 8f;
        shotgunDamage = 2f; //? vu que y'a plus de balles
        rifleDamage = 5f; //même raisonnement

        health = baseHealth;
        maxHealth = baseHealth;
        armure = baseArmure;
        maxArmure = baseArmure;

        damageUpgrade1 = false;
        damageUpgrade2 = false;
        damageUpgrade3 = false;

        healthUpgrade1 = false;
        healthUpgrade2 = false;
        healthUpgrade3 = false;

        armureUpgrade1 = false;
        armureUpgrade2 = false;
        armureUpgrade3 = false;

        poisoned = false;
        dead = false;
        recentlyFlung = false;
        switching = false;

        _healthText.text = "Vie : " + health + "/" + maxHealth;
        _armorText.text = "Armure : " + armure + "/" + maxArmure;
    }

    public void checkForUpgradeStation()
    {
        if (GameObject.Find("UpgradeDesk") != null)
            upgradeDeskScript = GameObject.Find("UpgradeDesk").GetComponent<UpgradeDeskScript>();
        else if (GameObject.Find("PodAlone") != null)
            upgradeDeskScript = GameObject.Find("PodAlone").GetComponent<UpgradeDeskScript>();
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

    public void hitmarkPlayer()
    {
        hitmark.PlayOneShot(HitmarkSound,0.3f);
    }

    private void shoot()
    {
        if (!switching)
        {
            switch (gunId)
            {
                case 0: //pistol
                    if (gunAnim.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("SwitchGun") &&
                        gunAnim.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).normalizedTime < 1.0f) { }
                    else
                    {
                        if (canShootPistol == 0)
                            StartCoroutine(PistolShot());
                    }
                    break;

                case 1: // shotgun
                    if (gunAnim.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("SwitchGun") &&
                        gunAnim.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).normalizedTime < 1.0f) { }
                    else
                    {
                        if (canShootShotgun == 0)
                            StartCoroutine(PumpShotgun());
                    }
                    break;

                case 2: // rifle
                    if (gunAnim.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("SwitchGun") &&
                       gunAnim.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).normalizedTime < 1.0f) { }
                    else
                    {
                        autoStop = true;
                        if (canShootRifle == 0)
                            StartCoroutine(AutomaticRifle());
                    }
                    break;
            }
        }
    }

    IEnumerator PistolShot()
    {
        canShootPistol = 1;
        GameObject bullet = Instantiate(_bullet, gunAnim.transform.Find("ShootPoint").position, _attach.rotation);
        if (!gunshotsMuted)
            audioSourcePistol.PlayOneShot(PistolSound);
        bullet.GetComponent<BulletScript>().setDamageParams(pistolDamage, damageUpgrade1, damageUpgrade2, damageUpgrade3);
        rb = bullet.GetComponent<Rigidbody>();
        rb.AddForce(_attach.forward * _force);
        yield return new WaitForSeconds(0.135f);
        canShootPistol = 0;
    }

    IEnumerator AutomaticRifle()
    {
        while (autoStop)
        {
            canShootRifle = 1;
            GameObject bullet = Instantiate(_bullet, gunAnim.transform.Find("ShootPoint").position, _attach.rotation);
            if (!gunshotsMuted)
                audioSourceLMG.PlayOneShot(LMGSound);
            bullet.GetComponent<BulletScript>().setDamageParams(rifleDamage, damageUpgrade1, damageUpgrade2, damageUpgrade3);
            rb = bullet.GetComponent<Rigidbody>();
            rb.AddForce(_attach.forward * _force);
            yield return new WaitForSeconds(0.09f);
            canShootRifle = 0;
        }
    }

    IEnumerator PumpShotgun()
    {
        canShootShotgun = 1;
        if (!gunshotsMuted)
            ShotgunSound.Play();
        for (int i = 0; i < 40; i++)
        {
            float randomX = Random.Range(-20f, 20f);
            float randomY = Random.Range(-20f, 20f);
            float randomZ = Random.Range(-20f, 20f);
            GameObject bullet = Instantiate(_bullet, gunAnim.transform.Find("ShootPoint").position, _attach.rotation);
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
        if (!switching)
            StartCoroutine(GunSwitch(gunId, 0));
    }

    private void OnGun2()
    {
        if (!switching)
            StartCoroutine(GunSwitch(gunId, 1));
    }

    private void OnGun3()
    {
        if (!switching)
            StartCoroutine(GunSwitch(gunId, 2));
    }

    IEnumerator GunSwitch(int currentGunId, int nextGunId)
    {
        if (currentGunId != nextGunId)
        {
            switching = true;

            GameObject gunHolder = gameObject.transform.Find("PlayerCameraRoot").Find("Attach").Find("GunHolder").gameObject;
            GameObject otherGun = gunHolder.transform.GetChild(nextGunId).gameObject;

            //current gun
            switch (currentGunId)
            {
                case 0: //pistol
                    gunAnim.GetComponent<Animator>().Play("SwitchPistol", -1, 0f);
                    break;

                case 1: //shotgun
                    gunAnim.GetComponent<Animator>().Play("SwitchShotgun", -1, 0f);
                    break;

                case 2: //rifle
                    gunAnim.GetComponent<Animator>().Play("SwitchRifle", -1, 0f);
                    break;

                default:
                    break;
            }

            //gun vers lequel on switch
            switch (nextGunId)
            {
                case 0: //pistol
                    otherGun.GetComponent<Animator>().Play("SwitchPistol", -1, 0f);
                    break;

                case 1: //shotgun
                    otherGun.GetComponent<Animator>().Play("SwitchShotgun", -1, 0f);
                    break;

                case 2: //rifle
                    otherGun.GetComponent<Animator>().Play("SwitchRifle", -1, 0f);
                    break;

                default:
                    break;
            }

            yield return new WaitForSeconds(0.4f);

            gunAnim.GetComponent<MeshRenderer>().enabled = false;
            gunAnim = otherGun;
            gunAnim.GetComponent<MeshRenderer>().enabled = true;

            gunId = nextGunId;

            switching = false;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 14)
        {
            if (other.gameObject.tag == "Money")
            {
                moneyScript = other.gameObject.GetComponent<MoneyScript>();
                money += moneyScript.getMoneyValue();
                _moneyText.text = "Credit : " + money.ToString();
                Destroy(other.gameObject);
                audioSourceMoney.PlayOneShot(MoneyPickUpSound);
                //Debug.Log("Money : " + money);
            }
            else if (other.gameObject.tag == "Helmet" || other.gameObject.tag == "Chestplate" || other.gameObject.tag == "Gloves" || other.gameObject.tag == "Boot" || other.gameObject.tag == "Health")
            {
                item = other.gameObject.GetComponent<Item>(); //chercher le script
                //Debug.Log("item" + item.getType());
                inventory.AddItem(item);

                GameObject itemObject = UIManager.GetComponent<UIManager>().AjoutItemUi(item);
                GameObject buttonDrop = itemObject.transform.GetChild(1).gameObject;
                GameObject buttonUse = itemObject.transform.GetChild(2).gameObject;
                GameObject inventoryManager = GameObject.Find("InventoryManager");
                Image theImage = itemObject.transform.GetChild(0).gameObject.GetComponent<Image>();
                buttonDrop.GetComponent<Button>().onClick.AddListener(delegate { inventoryManager.GetComponent<Inventory>().Drop(theImage); });
                buttonUse.GetComponent<Button>().onClick.AddListener(delegate { inventoryManager.GetComponent<Inventory>().Use(theImage); });
                audioSourceMoney.PlayOneShot(MoneyPickUpSound);
                Destroy(other.gameObject);
            }
        }
        else if (other.gameObject.layer == 9 && other.gameObject.name != "EnemyBulletIllusion(Clone)")
        {
            //mettre des if pour le nombre de degat recu
            switch (other.gameObject.name)
            {
                case "EnemyBullet(Clone)":
                    Damage(3);
                    break;
                case "EnemyBulletBig(Clone)":
                    Damage(10);
                    break;
                case "EnemyBulletPoison(Clone)":
                    if (!poisoned)
                        StartCoroutine(PoisonCoroutine());
                    break;
            }
            Destroy(other.gameObject);
        }
        else if(other.gameObject.tag == "Water")
        {
            waterRun = true;
            StartCoroutine(WaterCoroutine());
        }
        else
        {
            //Debug.Log("layer autre que collectible  : " + other.gameObject.layer);
        }
        if (other.gameObject.name == "tutoMusic")
        {
            PlayTutoMusic();
        }
        else if (other.gameObject.name == "combatMusic")
        {
            PlayCombatMusic();
        }
        else if (other.gameObject.name == "LobbyMusic")
        {
            PlayLobbyMusic();
        }
    }

    public void PlayTutoMusic()
    {
        if (CombatSound.isPlaying)
        {
            CombatSound.Stop();
        }
        if (LobbySound.isPlaying)
        {
            LobbySound.Stop();
        }
        if (DeathSound.isPlaying)
        {
            DeathSound.Stop();
        }
        if (!TutorialSound.isPlaying)
        {
            TutorialSound.Play();
        }
    }

    public void PlayCombatMusic()
    {
        if (TutorialSound.isPlaying)
        {
            TutorialSound.Stop();
        }
        if (LobbySound.isPlaying)
        {
            LobbySound.Stop();
        }
        if (DeathSound.isPlaying)
        {
            DeathSound.Stop();
        }
        if (!CombatSound.isPlaying)
        {
            CombatSound.Play();
        }
    }

    public void PlayLobbyMusic()
    {
        if (TutorialSound.isPlaying)
        {
            TutorialSound.Stop();
        }
        if (CombatSound.isPlaying)
        {
            CombatSound.Stop();
        }
        if (DeathSound.isPlaying)
        {
            DeathSound.Stop();
        }
        if (!LobbySound.isPlaying)
        {
            LobbySound.Play();
        }
    }

    public void PlayDeathMusic()
    {
        if (TutorialSound.isPlaying)
        {
            TutorialSound.Stop();
        }
        if (CombatSound.isPlaying)
        {
            CombatSound.Stop();
        }
        if (LobbySound.isPlaying)
        {
            LobbySound.Stop();
        }
        if (!DeathSound.isPlaying)
        {
            DeathSound.Play();
        }
    }

    public void StopMusic()
    {
        if (TutorialSound.isPlaying)
        {
            TutorialSound.Stop();
        }
        if (CombatSound.isPlaying)
        {
            CombatSound.Stop();
        }
        if (LobbySound.isPlaying)
        {
            LobbySound.Stop();
        }
        if (DeathSound.isPlaying)
        {
            DeathSound.Stop();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Water")
        {
            waterRun = false;
            StopCoroutine(WaterCoroutine());
        }
    }

    public void setMoney(int money)
    {
        _moneyText.text = "Credit : " + money.ToString();
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
                    case "ArmorChest": case "HealthChest": case "MoneyChest": case "ArmorChest(Clone)": case "HealthChest(Clone)": case "MoneyChest(Clone)":
                        if (hit.collider.gameObject.GetComponent<ChestScript>().isOpened() == false)
                            UIManager.GetComponent<UIManager>().Interactive(true, "Appuyez sur F pour ouvrir le coffre");
                        else
                            UIManager.GetComponent<UIManager>().Interactive(false, "");
                        break;
                    case "Door1_low":
                        if (getKey() == true && getLeLock() == false && getOnce() == true)
                            UIManager.GetComponent<UIManager>().Interactive(true, "Appuyez sur F pour ouvrir la porte");
                        else if(getKey() == false && getLeLock() == false)
                        {
                            UIManager.GetComponent<UIManager>().Interactive(true, "La porte est barrée");
                        }
                        break;
                    case "Door2_low":
                        if (getKey() == true && getLeLock() == false && getOnce() == true)
                            UIManager.GetComponent<UIManager>().Interactive(true, "Appuyez sur F pour ouvrir la porte");
                        else if (getKey() == false && getLeLock() == false)
                        {
                            UIManager.GetComponent<UIManager>().Interactive(true, "La porte est barrée");
                        }
                        break;
                    case "Key":
                        UIManager.GetComponent<UIManager>().Interactive(true, "Appuyez sur F pour prendre la clé");
                        break;
                    case "Teleporter":
                        UIManager.GetComponent<UIManager>().Interactive(true, "Appuyez sur F pour utiliser le téléporteur");
                        break;
                    case "Lock":
                        if (getKey() == false)
                        UIManager.GetComponent<UIManager>().Interactive(true, "La porte est barrée");
                        else if (getKey() == true)
                            UIManager.GetComponent<UIManager>().Interactive(true, "Appuyez sur F pour débarrer la porte");
                        break;
                    case "Bed":
                        UIManager.GetComponent<UIManager>().Interactive(true, "Appuyer sur F pour dormir (Quitter la partie)");
                        break;
                    case "MissionCrate":
                        UIManager.GetComponent<UIManager>().Interactive(true, "Appuyer sur F pour partir à l'aventure !");
                        break;
                    case "PodInteraction":
                        UIManager.GetComponent<UIManager>().Interactive(true, "Appuyer sur F pour parcourir les améliorations");
                        break;
                    default:
                        UIManager.GetComponent<UIManager>().Interactive(true, "Appuyez sur F pour intéragir");
                        break;
                }

                if (interactable == null || interactable.ID != hit.collider.GetComponent<Interactable>().ID)
                {
                    interaction = true;
                    interactable = hit.collider.GetComponent<Interactable>();
                    //Debug.Log("new interactable " + interactable);
                }
                else
                {
                    interaction = false;
                    //Debug.Log("new interactable " + interactable);
                }
            }
        }
        else
        {
            UIManager.GetComponent<UIManager>().Interactive(false, "");
        }

        if (firstPersonController.onEnemy && recentlyFlung == false)
        {
            StartCoroutine(FlingPlayer());
        }
        
    }

    public void Damage(int damage)
    {
        if (!dead)
        {
            int damageToHealth = 0;
            getRandomGrunt();
            if (armure > 0)
            {
                armure -= damage;
                damageToHealth = -(armure);
                if (armure <= 0)
                {
                    ShieldCrackSound.Play();
                    armure = 0;
                }
            }
            else
                damageToHealth = damage;

            if (armure <= 0)
            {
                health -= damageToHealth;

                if (health <= 0)
                {
                    dead = true;

                    health = maxHealth;
                    armure = maxArmure;
                    PlayDeathMusic();
                    crossHair.SetActive(false);
                    _healthText.gameObject.SetActive(false);
                    _armorText.gameObject.SetActive(false);
                    UIManager.GetComponent<UIManager>().DeathScreen(true);
                }
            }
            _healthText.text = "Vie : " + health + "/" + maxHealth;
            _armorText.text = "Armure : " + armure + "/" + maxArmure;
            //Debug.Log("armure: " + armure + ", health: " + health);
        }
    }

    public void Respawn()
    {
        if (UpdateSpawn != null)
        {
            onOffSpawn = UpdateSpawn.GetComponent<UpdateSpawn>().getOnOff();
            if (onOffSpawn == false)
            {
                gameObject.transform.SetPositionAndRotation(TutoSpawn.position, TutoSpawn.rotation);
            }
            if (onOffSpawn == true)
            {
                gameObject.transform.SetPositionAndRotation(LobbySpawn.position, LobbySpawn.rotation);
            }
            health = maxHealth;
            armure = maxArmure;
            dead = false;
        }
        else
        {
            //l'envoyer à l'autre scène
            returned = true;
            dead = false;
            GameObject.Find("UI").GetComponent<IdentifyUI>().returned = true;

            GameObject.Find("SceneManager").GetComponent<SceneControl>().returnToLobby();
        }
    }

    private void getRandomGrunt()
    {
        int nbr = Random.Range(1, 4);

        switch (nbr)
        {
            case 1:
                GruntSound1.Play();
                break;
            case 2:
                GruntSound2.Play();
                break;
            case 3:
                GruntSound3.Play();
                break;
            default:
                break;
        }
    }

    IEnumerator PoisonCoroutine()
    {
        poisoned = true;

        yield return new WaitForSeconds(0.5f);
        Damage(5);
        yield return new WaitForSeconds(1.0f);
        Damage(5);
        yield return new WaitForSeconds(1.0f);
        Damage(5);

        poisoned = false;
    }

    IEnumerator WaterCoroutine()
    {
        while (waterRun == true)
        {
            Damage(10);
            yield return new WaitForSeconds(0.5f);
        }
    }

    public void setHealthUpgrades(bool _healthUpgrade1, bool _healthUpgrade2, bool _healthUpgrade3)
    {
        healthUpgrade1 = _healthUpgrade1;
        healthUpgrade2 = _healthUpgrade2;
        healthUpgrade3 = _healthUpgrade3;
        //Debug.Log("health upgrades updated: " + healthUpgrade1 + " " + healthUpgrade2 + " " + healthUpgrade3);

        health = baseHealth; //heal to 100
        if (healthUpgrade1)
        {
            health = baseHealth + 100;
        }
        if (healthUpgrade2)
        {
            health += 100;
        }
        if (healthUpgrade3)
        {
            health += 100;
        }
        maxHealth = health;
        _healthText.text = "Vie : " + health + "/" + maxHealth;
        //Debug.Log("new health: " + health);
    }

    public void setArmureUpgrades(bool _armureUpgrade1, bool _armureUpgrade2, bool _armureUpgrade3)
    {
        armureUpgrade1 = _armureUpgrade1;
        armureUpgrade2 = _armureUpgrade2;
        armureUpgrade3 = _armureUpgrade3;
        //Debug.Log("armure upgrades updated: " + armureUpgrade1 + " " + armureUpgrade2 + " " + armureUpgrade3);

        armure = baseArmure; //set to 100 (??)
        if (armureUpgrade1)
        {
            armure = baseArmure + 100;
        }
        if (armureUpgrade2)
        {
            armure += 100;
        }
        if (armureUpgrade3)
        {
            armure += 100;
        }
        maxArmure = armure;
        _armorText.text = "Armure : " + armure + "/" + maxArmure;
        //Debug.Log("new armure: " + armure);
    }

    public void OnInteract() // le boutons
    {
        if(interactable == true)
        {
            RaycastHit hit;
            if (Physics.Raycast(camera.transform.position, camera.transform.forward, out hit, 2, interactableLayerMask))
            {
                if (hit.collider.GetComponent<Interactable>() != false)
                {
                    interactable.onInteract.Invoke();
                }
            }
        }
        else
        {
            //Debug.Log("/Vegu");
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
        //Debug.Log(onOffCrouch);
        if (!onOffCrouch)
        {
            /*_CharacterController.height = 1.0f;
            capsuleCollider.height = 1.0f;
            firstPersonController.GroundedOffset = -1.0f;
            onOffCrouch = true;*/
        StartCoroutine(Start3());
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
        for (float f = 1; f <= 2; f += 0.04f)
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

    IEnumerator Start3()
    {
        for (float f = 2; f >= 1; f -= 0.04f)
        {
            _CharacterController.height = f;
            capsuleCollider.height = f;
            if (f >= 1.999)
            {
                _CharacterController.height = 1f;
                capsuleCollider.height = 1f;
            }
            yield return new WaitForSeconds(0.001f);
        }
    }

    //check for player standing on enemy. if true, fling backwards
    IEnumerator FlingPlayer()
    {
        //fling player
        Vector3 flingDirection = new Vector3(Random.Range(-1f, 1f), 1f, Random.Range(-1f, 1f));
        flingPlayerScript.AddImpact(flingDirection, 60f);
        Damage(5);

        //cooldown
        recentlyFlung = true;
        yield return new WaitForSeconds(0.5f);
        recentlyFlung = false;
    }


    public void addHealth(int addedHealth)
    {
        int add = health + addedHealth;
        if(add > maxHealth)
        {
            health = maxHealth;
            _healthText.text = "Vie : " + health + "/" + maxHealth;
        }
        else
        {
            health = add;
            _healthText.text = "Vie : " + health + "/" + maxHealth;
        }
        HPRegenSound.Play();
    }

    public void addArmor(int addedArmor)
    {
        int add = armure + addedArmor;
        if (add > maxArmure)
        {
            armure = maxArmure;
            _armorText.text = "Armure : " + armure + "/" + maxArmure;
        }
        else
        {
            armure = add;
            _armorText.text = "Armure : " + armure + "/" + maxArmure;
        }
        ShieldRegenSound.Play();
    }

    public bool getKey()
    {
        return key;
    }

    public void setKey(bool _key)
    {
        if (!onceAgain)
        {
            MoneyPickUpSoundSource.Play();
            onceAgain = true;
        }
        key = _key;
    }

    public bool getLeLock()
    {
        return LeLock;
    }

    public void setLeLock(bool _LeLock)
    {
        LeLock = _LeLock;
    }

    public bool getOnce()
    {
        return LeLock;
    }

    public void setOnce(bool _Once)
    {
        once = _Once;
    }
}