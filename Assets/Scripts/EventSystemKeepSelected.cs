using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class EventSystemKeepSelected : MonoBehaviour
{
    private EventSystem eventSystem;
    private GameObject keptSelection = null;
    void Start()
    {
        eventSystem = GetComponent<EventSystem>();
    }

    void Update()
    {
        //pour le screen upgrade uniquement (for now)
        if (GameObject.Find("UpgradeDesk") != null)
        {
            if (GameObject.Find("UpgradeDesk").GetComponent<UpgradeDeskScript>().checkIfActive() == true)
            {

                /*
                if (eventSystem.currentSelectedGameObject != null)
                {
                    if (eventSystem.currentSelectedGameObject.GetComponent<UpgradeScript>() != null)
                    {
                        keptSelection = eventSystem.currentSelectedGameObject;
                    }
                }

                if (keptSelection != null)
                {
                    eventSystem.SetSelectedGameObject(GameObject.Find(keptSelection.name));
                    Debug.Log(keptSelection.name);
                }
                */

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
    }
}
