using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Inventory : MonoBehaviour
{
    [SerializeField] GameObject helmet;
    [SerializeField] GameObject chestplate;
    [SerializeField] GameObject gloves;
    [SerializeField] GameObject boots;
    [SerializeField] GameObject health;
    private List<Item> itemList;

    public Inventory()
    {
        itemList = new List<Item>();
        //AddItem(new Item { itemType = Item.ItemType.Chest, amount = 1 });
        // toute les items différents devront être lister ci-dessous
    }

    public void AddItem(Item item)
    {
        itemList.Add(item);
    }

    public void RemoveItem(Item item)
    {/*
        foreach(sprites in List)
        {
            if (counter == 0)
            {
                if (nom in list)
                {
                    delete;
                    counter++;
                }
            }
            break;
        }*/
    }

    public List<Item> GetItemList()
    {
        return itemList;
    }

    public void Drop(Image image)
    {
        string objectName = image.sprite.name;
        Transform playerPosition = GameManager.PlayerPostion;
        switch (objectName)
        {
            case "Helmet":
                Instantiate(helmet, playerPosition.position, playerPosition.rotation);
                Debug.Log("player pos : " + playerPosition);
                break;

            case "Chestplate":
                Instantiate(chestplate, playerPosition.position, playerPosition.rotation);
                Debug.Log("player pos : " + playerPosition);
                break;
            case "Gloves":

                break;

            case "Boot":
                break;

            case "Health":
                Instantiate(health, playerPosition.position, playerPosition.rotation);
                Debug.Log("player pos : " + playerPosition);
                break;

            default:

                break;
        }
        Debug.Log(objectName);
    }
    
}
