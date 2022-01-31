using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    private bool pause = false;
    [SerializeField] private GameObject _escapeMenuPanel;

    public void PauseGame()
    {
        if (Time.timeScale == 1)
        {
            pause = true;
            Time.timeScale = 0;
            _escapeMenuPanel.SetActive(true);
        }
        else if (Time.timeScale == 0)
        {
            Time.timeScale = 1;
            pause = false;
            _escapeMenuPanel.SetActive(false);
        }
    }

    void Start()
    {
        
    }

    
    void Update()
    {
        
    }
}
