using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    public bool pause = false;
    private bool OnOff;
    [SerializeField] private GameObject _escapeMenuPanel;
    [SerializeField] private GameObject _interact;
    [SerializeField] private Transform _itemSlotContainer;
    [SerializeField] private GameObject _item;
    [SerializeField] private Sprite[] _itemSprite;
    [SerializeField] private GameObject[] _itemList;

    public void PauseGame()
    {
        if (Time.timeScale == 1)
        {
            pause = true;
            Time.timeScale = 0;
            _escapeMenuPanel.SetActive(true);
            CursorUnlock();
        }
        else if (Time.timeScale == 0)
        {
            Time.timeScale = 1;
            pause = false;
            _escapeMenuPanel.SetActive(false);
            CursorLock();
            
        }
    }

    public void Interactive(bool OnOff, string message)
    {
        switch (OnOff)
        {
            case true:
                //Debug.Log(_interact.name);
                _interact.SetActive(true);
                _interact.GetComponent<TMP_Text>().SetText(message);
                break;

            case false:
                _interact.SetActive(false);
                break;
        }
        
    }

    public void AjoutItemUi(Item i)
    {
        GameObject inventoryItem = Instantiate(_item, _itemSlotContainer);
        Image image = inventoryItem.transform.GetChild(0).gameObject.GetComponent<Image>();
        switch (i.getType()){
            case "Helmet":
                // something changer la source de l'image
                break;
            case "Chest":
               // i = _itemList[0]; // jpense que sa va finir par planter -> un moment donner, s'il y a plus que un item avec des stats differentes,
                image.sprite = _itemSprite[0];
                break;
            case "Gloves":
                
                break;
            case "Boot":
                break;
            case "Health":
               // i = _itemList[1];
                image.sprite = _itemSprite[1];
                break;

            default:

                break;
        }
    }

    public void DropItem()
    {
       
    }

    public bool getPause()
    {
        return pause;
    }

    public void CursorUnlock()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public void CursorLock()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }
}
