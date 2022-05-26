using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using StarterAssets;

public class SettingsScript : MonoBehaviour
{
    [SerializeField] Slider volumeSlider;
    [SerializeField] Slider sensitivitySlider;

    // Start is called before the first frame update
    void Start()
    {
        if (!PlayerPrefs.HasKey("masterVolume"))
        {
            PlayerPrefs.SetFloat("masterVolume", 1);
        }
        if (!PlayerPrefs.HasKey("sensitivity"))
        {
            PlayerPrefs.SetFloat("sensitivity", 1);
            Debug.Log("key not found");
        }

        Load();
    }

    public void ChangeVolume()
    {
        AudioListener.volume = volumeSlider.value;
        PlayerPrefs.SetFloat("masterVolume", volumeSlider.value);
    }

    public void ChangeSensitivity()
    {
        if (GameObject.Find("Player") != null)
        {
            GameObject player = GameObject.Find("Player");
            FirstPersonController fpsCont = player.GetComponent<FirstPersonController>();
            fpsCont.RotationSpeed = sensitivitySlider.value;
        }
        PlayerPrefs.SetFloat("sensitivity", sensitivitySlider.value);
    }

    private void Load()
    {
        Debug.Log("loadz");
        volumeSlider.value = PlayerPrefs.GetFloat("masterVolume");
        sensitivitySlider.value = PlayerPrefs.GetFloat("sensitivity");
    }

}
