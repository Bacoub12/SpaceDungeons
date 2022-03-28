using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    public bool pause = false;
    private bool OnOff;
    public Sprite sprite;
    [SerializeField] private GameObject _escapeMenuPanel;
    [SerializeField] private GameObject _interact;
    [SerializeField] private GameObject _deathPanel;
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

    public void DeathScreen(bool OnOff)
    {
        switch (OnOff)
        {
            case true:
                //Debug.Log(_interact.name);
                _deathPanel.SetActive(true);
                CursorUnlock();
                //Time.timeScale = 0; 
                break;

            case false:
                _deathPanel = GameObject.Find("DeathPanel");
                _deathPanel.SetActive(false);
                CursorLock();
                //Time.timeScale = 1;
                break;
        }
    }

    public GameObject AjoutItemUi(Item i)
    {
        GameObject inventoryItem = Instantiate(_item, _itemSlotContainer);
        Image image = inventoryItem.transform.GetChild(0).gameObject.GetComponent<Image>();
        switch (i.getType()){
            case "Helmet":
                image.sprite = _itemSprite[0];
                break;
            case "Chestplate":
                image.sprite = _itemSprite[1];
                break;
            case "Gloves":
                image.sprite = _itemSprite[2];
                break;
            case "Boot":
                image.sprite = _itemSprite[3];
                break;
            case "Health":
                image.sprite = _itemSprite[4];
                break;

            default:

                break;
        }

        return inventoryItem;
    }

    public void DropItem()
    {
        sprite = this.gameObject.GetComponentInChildren<Image>().sprite;
        Debug.Log("sprite : " + sprite);
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
