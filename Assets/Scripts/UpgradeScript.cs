using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UpgradeScript : MonoBehaviour
{
    public string id, title, description;
    public List<string> dependsOn;
    public float price;
    public bool bought;
}
