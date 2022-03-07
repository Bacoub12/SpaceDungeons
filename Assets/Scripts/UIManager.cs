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
                image.sprite = _itemSprite[0];
                break;
            case "Pantalon":
                image.sprite = _itemSprite[1];
                break;
            case "Boot":
                break;

            default:

                break;
        }
        
        
        
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
