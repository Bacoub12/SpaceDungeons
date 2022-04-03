using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class resetFloorNumberScript : MonoBehaviour
{
    int floorNumber = 0;
    TMP_Text floorText;

    public void Start()
    {
        floorText = GameObject.Find("FloorText").GetComponent<TMP_Text>();
    }

    public void ResetNumber()
    {
        floorNumber = 1;
        floorText.text = "salle : " + floorNumber;
    }

    public int Increase()
    {
        floorNumber += 1;
        return floorNumber;
    }

    public int getFloorNumber()
    {
        return floorNumber;
    }

    public void setFloorNumber(int _floorNumber)
    {
        floorNumber = _floorNumber;
    }
    
}
