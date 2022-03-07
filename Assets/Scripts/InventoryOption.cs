using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryOption : MonoBehaviour
{
    private bool _active = false;
    [SerializeField] public GameObject _inventaireOption;
    GameObject menu;
    public void isActivated()
    {
        if(_active == false)
        {
            menu = Instantiate(_inventaireOption, this.gameObject.transform);
            _active = true;
        }
        else if(_active == true)
        {
            Destroy(menu);
            _active = false;
        }
    }
}
