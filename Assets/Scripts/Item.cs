using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    [SerializeField] public string type;
    [SerializeField] public int amount;


    public Item(string type, int amount)
    {
        this.type = type;
        this.amount = amount;
    }

    public void setType(string _type)
    {
        type = _type;
    }

    public string getType()
    {
        return type;
    }

    public void setAmount(int _amount)
    {
        amount = _amount;
    }

    public int getAmount()
    {
        return amount;
    }

}
