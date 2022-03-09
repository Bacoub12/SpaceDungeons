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

    public void RemoveItem(Image image)
    {
        string name = image.sprite.name;
        bool found = false;
        foreach(Item item in itemList) /* work in progress*/
        {
            if (found == false)
            {
                if (name == item.getType())
                {
                    Destroy(image.gameObject.transform.parent.gameObject);
                    found = true;
                }
            }
            break;
        }
    }

    public List<Item> GetItemList()
    {
        return itemList;
    }

    public void Drop(Image image)
    {
        string objectName = image.sprite.name;
        Transform playerPosition = GameManager.PlayerPostion;
        Vector3 launch = new Vector3(playerPosition.position.x, playerPosition.position.y + 2f, playerPosition.position.z);
        Vector3 forward = playerPosition.forward;
        switch (objectName)
        {
            case "Helmet":
                Instantiate(helmet, launch + forward, playerPosition.rotation);
                break;

            case "Chestplate":
                Instantiate(chestplate, launch + forward, playerPosition.rotation);
                break;
            case "Gloves":

                break;

            case "Boot":
                break;

            case "Health":

                Instantiate(health, launch + forward, playerPosition.rotation);
                break;

            default:

                break;
        }
    }

    public void use(Image image)
    {
        string objectName = image.sprite.name;
        switch (objectName)
        {
            case "Helmet":
                break;

            case "Chestplate":
                break;
            case "Gloves":

                break;

            case "Boot":
                //use armor
                break;

            case "Health":
                //use health
                break;

            default:

                break;
        }
    }
    
}
