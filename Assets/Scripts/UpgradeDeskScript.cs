using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UpgradeDeskScript : MonoBehaviour
{
    public GameObject upgradeDeskPanel;
    public GameObject uiManager;

    private bool isActive, initialized;

    // Start is called before the first frame update
    void Start()
    {
        isActive = false;
        initialized = false;
    }

    // Update is called once per frame
    void Update()
    {
        
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

                        /*
                        string objectName = buttonGameObject.name;
                        switch (objectName)
                        {
                            case "btUpgradeDegats1":
                                upgradeScript.setUpgradeInfo("Dégâts I", "Envéloppe vos balles de cyanure.", 200);
                                break;
                            case "btUpgradeDegats2":
                                upgradeScript.setUpgradeInfo("Dégâts II", "Envéloppe vos balles de cyanure.", 300);
                                break;
                            case "btUpgradeDegats3":
                                upgradeScript.setUpgradeInfo("Dégâts III", "Envéloppe vos balles de cyanure.", 400);
                                break;
                        }
                        */

                        //chaque bouton doit déclencher dans son onclick un load basé sur le script UpgradeScript à côté de lui
                        buttonGameObject.GetComponent<Button>()
                            .onClick.AddListener(delegate {
                                loadUpgradeInfo(upgradeScript.title, upgradeScript.description, upgradeScript.price); 
                            });
                    }
                    

                    initialized = true;
                }
            }
        }
    }

    public void loadUpgradeInfo(string title, string description, float price)
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
