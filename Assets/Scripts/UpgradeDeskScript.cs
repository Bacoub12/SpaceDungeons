using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class UpgradeDeskScript : MonoBehaviour
{
    public GameObject upgradeDeskPanel;
    public GameObject uiManager;

    private bool isActive, initialized;
    private Color boughtColor;
    private EventSystem eventSystem;

    // Start is called before the first frame update
    void Start()
    {
        isActive = false;
        initialized = false;
        boughtColor = new Color(0.71f, 0.92f, 0.35f, 1f);
        eventSystem = GameObject.Find("EventSystem").GetComponent<EventSystem>();
    }

    // Update is called once per frame
    void Update()
    {
        if (checkIfActive() == true)
        {
            string upgradeName = GameObject.Find("textTitle").GetComponent<TMP_Text>().text;
            foreach (Transform child in GameObject.Find("ListPanel").transform)
            {
                GameObject buttonGameObject = child.gameObject;
                if (buttonGameObject.transform.GetChild(0).gameObject.GetComponent<TMP_Text>().text == upgradeName)
                {
                    if (eventSystem.currentSelectedGameObject == buttonGameObject)
                        eventSystem.SetSelectedGameObject(null);
                    eventSystem.SetSelectedGameObject(buttonGameObject);
                }
            }
        }
    }

    public void toggleUpgradeInterface()
    {
        if (!uiManager.GetComponent<UIManager>().getPause())
        {
            if (isActive)
            {
                upgradeDeskPanel.SetActive(false);
                Time.timeScale = 1;
                CursorLock();
                isActive = false;
            }
            else
            {
                upgradeDeskPanel.SetActive(true);
                Time.timeScale = 0;
                CursorUnlock();
                isActive = true;

                if (initialized == false)
                {
                    
                    foreach (Transform child in GameObject.Find("ListPanel").transform)
                    {
                        GameObject buttonGameObject = child.gameObject;
                        UpgradeScript upgradeScript = buttonGameObject.GetComponent<UpgradeScript>();

                        buttonGameObject.GetComponent<Button>()
                            .onClick.AddListener(delegate {
                                loadUpgradeInfo(upgradeScript.title, upgradeScript.description, upgradeScript.price, upgradeScript.bought); 
                            });

                        if (upgradeScript.bought)
                            buttonGameObject.GetComponent<Image>().color = boughtColor;
                    }

                    GameObject buyButton = GameObject.Find("btBuy");
                    buyButton.GetComponent<Button>()
                            .onClick.AddListener(delegate {
                                buyUpgrade();
                                updateMoneyVisual();
                            });

                    initialized = true;
                }

                updateMoneyVisual();

                GameObject descPanel = GameObject.Find("DescPanel");
                descPanel.transform.GetChild(0).gameObject
                    .GetComponent<TMP_Text>()
                    .text = "";
                descPanel.transform.GetChild(1).gameObject
                    .GetComponent<TMP_Text>()
                    .text = "";
                descPanel.transform.GetChild(2).gameObject
                    .GetComponent<TMP_Text>()
                    .text = "";
                descPanel.transform.GetChild(3).gameObject
                    .GetComponent<Button>()
                    .interactable = false;
            }
        }
    }

    public void updateMoneyVisual()
    {
        GameObject moneyText = GameObject.Find("MoneyText");
        moneyText.GetComponent<TMP_Text>().text = "Crédits: " + GameObject.Find("PlayerCapsule").GetComponent<PlayerScript>().money;
    }

    public void loadUpgradeInfo(string title, string description, float price, bool bought)
    {
        GameObject descPanel = GameObject.Find("DescPanel");
        descPanel.transform.GetChild(0).gameObject
            .GetComponent<TMP_Text>()
            .text = title;
        descPanel.transform.GetChild(1).gameObject
            .GetComponent<TMP_Text>()
            .text = description;
        descPanel.transform.GetChild(2).gameObject
            .GetComponent<TMP_Text>()
            .text = "Prix: " + price;

        if (bought)
        {
            descPanel.transform.GetChild(3).gameObject
                .GetComponent<Button>()
                .interactable = false;
        }
        else
        {
            descPanel.transform.GetChild(3).gameObject
                .GetComponent<Button>()
                .interactable = true;
        }
    }

    public void buyUpgrade()
    {
        int price = int.Parse(GameObject.Find("textPrice").GetComponent<TMP_Text>().text.Split(':')[1].Trim());
        int money = int.Parse(GameObject.Find("MoneyText").GetComponent<TMP_Text>().text.Split(':')[1].Trim());

        if (money > price)
        {
            int newMoney = money - price;
            GameObject.Find("PlayerCapsule").GetComponent<PlayerScript>().money = newMoney;
            updateMoneyVisual();

            string upgradeName = GameObject.Find("textTitle").GetComponent<TMP_Text>().text;
            foreach (Transform child in GameObject.Find("ListPanel").transform)
            {
                GameObject buttonGameObject = child.gameObject;
                if (buttonGameObject.transform.GetChild(0).gameObject.GetComponent<TMP_Text>().text == upgradeName)
                {
                    UpgradeScript upgradeScript = buttonGameObject.GetComponent<UpgradeScript>();
                    upgradeScript.bought = true;
                    buttonGameObject.GetComponent<Image>().color = boughtColor;
                }
            }

            GameObject.Find("DescPanel")
                .transform.GetChild(3)
                .gameObject.GetComponent<Button>()
                .interactable = false;

            switch (upgradeName.Split(' ')[0])
            {
                case "Dégâts":
                    updateDmgUpgrades();
                    break;
                case "Santé":
                    updateHealthUpgrades();
                    break;
                case "Armure":
                    updateArmorUpgrades();
                    break;
            }
        }
    }

    private void updateDmgUpgrades()
    {
        bool dmgUpgrade1 = false;
        bool dmgUpgrade2 = false;
        bool dmgUpgrade3 = false;
        PlayerScript playerScript = GameObject.Find("PlayerCapsule").GetComponent<PlayerScript>();
        if (GameObject.Find("btUpgradeDegats1").GetComponent<Image>().color == boughtColor)
            dmgUpgrade1 = true;
        if (GameObject.Find("btUpgradeDegats2").GetComponent<Image>().color == boughtColor)
            dmgUpgrade2 = true;
        if (GameObject.Find("btUpgradeDegats3").GetComponent<Image>().color == boughtColor)
            dmgUpgrade3 = true;
        playerScript.setDamageUpgrades(dmgUpgrade1, dmgUpgrade2, dmgUpgrade3);
    }

    private void updateHealthUpgrades()
    {
        bool healthUpgrade1 = false;
        bool healthUpgrade2 = false;
        bool healthUpgrade3 = false;
        PlayerScript playerScript = GameObject.Find("PlayerCapsule").GetComponent<PlayerScript>();
        if (GameObject.Find("btUpgradeSante1").GetComponent<Image>().color == boughtColor)
            healthUpgrade1 = true;
        if (GameObject.Find("btUpgradeSante2").GetComponent<Image>().color == boughtColor)
            healthUpgrade2 = true;
        if (GameObject.Find("btUpgradeSante3").GetComponent<Image>().color == boughtColor)
            healthUpgrade3 = true;
        playerScript.setHealthUpgrades(healthUpgrade1, healthUpgrade2, healthUpgrade3);
    }

    private void updateArmorUpgrades()
    {
        bool armorUpgrade1 = false;
        bool armorUpgrade2 = false;
        bool armorUpgrade3 = false;
        PlayerScript playerScript = GameObject.Find("PlayerCapsule").GetComponent<PlayerScript>();
        if (GameObject.Find("btUpgradeArmure1").GetComponent<Image>().color == boughtColor)
            armorUpgrade1 = true;
        if (GameObject.Find("btUpgradeArmure2").GetComponent<Image>().color == boughtColor)
            armorUpgrade2 = true;
        if (GameObject.Find("btUpgradeArmure3").GetComponent<Image>().color == boughtColor)
            armorUpgrade3 = true;
        playerScript.setArmureUpgrades(armorUpgrade1, armorUpgrade2, armorUpgrade3); //todo faire les upgrades d'armure quand raph aura fait l'armure en soi
    }

    private void CursorUnlock()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    private void CursorLock()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    public bool checkIfActive()
    {
        return isActive;
    }
}
