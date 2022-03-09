using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    int id;
    [SerializeField] public string type;


    public Item(string type, int id)
    {
        this.type = type;
        this.id = id;
    }

    public void setType(string _type)
    {
        type = _type;
    }

    public string getType()
    {
        return type;
    }

    public void setId(int _id)
    {
        id = _id;
    }

    public int getId()
    {
        return id;
    }

}
