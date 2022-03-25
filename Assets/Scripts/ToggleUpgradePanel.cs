using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleUpgradePanel : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void toggle()
    {
        if (GameObject.Find("UpgradeDesk"))
        {
            GameObject.Find("UpgradeDesk").GetComponent<UpgradeDeskScript>().toggleUpgradeInterface();
        }
        else if (GameObject.Find("PodAlone"))
        {
            GameObject.Find("PodAlone").GetComponent<UpgradeDeskScript>().toggleUpgradeInterface();
        }
    }
}
