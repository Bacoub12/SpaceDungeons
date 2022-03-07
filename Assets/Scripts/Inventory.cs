using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{

    private List<Item> itemList;

    public Inventory()
    {
        itemList = new List<Item>();
        //AddItem(new Item { itemType = Item.ItemType.Chest, amount = 1 });
        // toute les items diff�rents devront �tre lister ci-dessous
    }

    public void AddItem(Item item)
    {
        itemList.Add(item);
    }

    public void RemoveItem(Item item)
    {
        //avec le menu quand on click sur drop
    }

    public List<Item> GetItemList()
    {
        return itemList;
    }
    
}
