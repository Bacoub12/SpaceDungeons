using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonScript : MonoBehaviour
{
    public AudioSource mySource;
    public AudioClip click;
    public AudioClip hover;

    public void HoverSound()
    {
        mySource.PlayOneShot(hover);
    }

    public void ClickSound()
    {
        mySource.PlayOneShot(click);
    }

    public void getReturnToLobby()
    {
        GameObject.Find("ExitZoneFinal(Clone)").GetComponent<ExitZoneScript>().ReturnToLobby();
    }
}
