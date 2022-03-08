using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryOption : MonoBehaviour
{
    public GameObject _inventaireOption;
    public Button _btEquip;
    public Button _btDrop;

    public void Start()
    {
        _inventaireOption = GameObject.Find("InventoryOption");
        _btEquip = GameObject.Find("btEquip").GetComponent<Button>();
        _btDrop = GameObject.Find("btDrop").GetComponent<Button>();
    }

    /*    Si on veut ajouter d'autres items dans l'inventaire qui ont des options qui ne sont pas equip ou drop, ici est l'emplacement ou on va pouvoir controler
     *    les diseable et les enable des boutons.*/
    public void isActivated()
    {
        /*if(_inventaireOption.activeSelf == false)
        {
            _inventaireOption.SetActive(true);
        }
        else if(_inventaireOption.activeSelf == true)
        {
            _inventaireOption.SetActive(false);
        }*/
    }
    public void Activated()
    {
        _btEquip = GameObject.Find("btEquip").GetComponent<Button>();
        _btDrop = GameObject.Find("btDrop").GetComponent<Button>();
        _btEquip.interactable = true;
        _btDrop.interactable = true;
    }

    public void Deactivated()
    {
        _btEquip.interactable = false;
        _btDrop.interactable = false;
    }
}
