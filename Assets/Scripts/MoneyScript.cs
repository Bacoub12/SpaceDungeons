using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoneyScript : MonoBehaviour
{
    [SerializeField] public int moneyValue;
    
    public int getMoneyValue()
    {
        return moneyValue;
    }
}
