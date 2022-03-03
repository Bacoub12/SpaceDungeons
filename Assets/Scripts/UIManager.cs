using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
    public bool pause = false;
    private bool OnOff;
    [SerializeField] private GameObject _escapeMenuPanel;
    [SerializeField] private GameObject _interact;
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
