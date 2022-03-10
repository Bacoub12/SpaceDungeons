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

    private void Start()
    {
        itemList = new List<Item>();
        Debug.Log("lists initialized");
    }

    public void AddItem(Item item)
    {
        itemList.Add(item);
        Debug.Log("item added, item count now " + itemList.Count);
        if (itemList.Count == 2)
        {
            Debug.Log("oui");
        }
        

    }

    public void RemoveItem(Image image)
    {
        //Debug.Log("it pass");
        string name = image.sprite.name;
        bool found = false;
        int removedItemPosition = 0;

        Debug.Log("trying to remove, item count now " + itemList.Count);
        foreach (Item item in itemList) /* work in progress*/
        {
            Debug.Log("dans le foreacj");
            if (found == false)
            {
                Debug.Log("dans le if, avec spriteName " + name + " et item type" + item.getType());
                if (name == item.getType())
                {
                    Destroy(image.gameObject.transform.parent.gameObject);
                    removedItemPosition = itemList.IndexOf(item);
                    Debug.Log("dans le if2");
                    found = true;
                }
            }
        }

        itemList.RemoveAt(removedItemPosition);
    }

    public List<Item> GetItemList()
    {
        return itemList;
    }

    public void Drop(Image image)
    {

        Debug.Log("trying to drop, item count now " + itemList.Count);
        string objectName = image.sprite.name;
        Transform playerPosition = GameManager.PlayerPostion;
        Vector3 launch = new Vector3(playerPosition.position.x, playerPosition.position.y + 0.5f, playerPosition.position.z);
        Vector3 forward = playerPosition.forward *1.5f;
        switch (objectName)
        {
            case "Helmet":
                Instantiate(helmet, launch + forward, playerPosition.rotation);
                break;

            case "Chestplate":
                Instantiate(chestplate, launch + forward, playerPosition.rotation);
                RemoveItem(image);
                break;
            case "Gloves":

                break;

            case "Boot":
                break;

            case "Health":

                Instantiate(health, launch + forward, playerPosition.rotation);
                RemoveItem(image);
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
