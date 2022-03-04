using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    [SerializeField] public string type;


    public Item(string type)
    {
        this.type = type;
    }

    public void setType(string _type)
    {
        type = _type;
    }

    public string getType()
    {
        return type;
    }

}
