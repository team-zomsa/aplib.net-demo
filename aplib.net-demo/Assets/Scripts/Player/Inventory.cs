using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    //TODO Add icon logic to Item class
    //TODO Add test Icon to TestItem class
    //TODO Add hotbar Object which contains 4 subjobjects. They are linked to the inventory class and will display the right icons depending on inventory contents.
    //TODO Add controls for using items to InputManager(?) class
    //TODO Add controls for switching which item you're using to InputManager(?) class
    //TODO Couple this script to the player
    Queue<Item> itemList;
    public float inventorySize;
    // Start is called before the first frame update
    void Start()
    {
        itemList = new Queue<Item>();
        inventorySize = 4;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    /// <summary>
    /// Converts queue to list to check if there are any items with matching names. If there are it checks if they are stackable and adds uses. If they are not it does nothing. If there are not matching names it adds the item to the inventory;
    /// </summary>
    /// <param name="item"></param>
    /// <param name="uses"></param>
    public void PickUpItem(Item item, float uses = 1)
    {
        float queueSize = itemList.Count();
        bool alreadyInInventory=false;

            for (int i = 0; i < queueSize; i++) {
            if (itemList.ToList()[i].itemName == item.itemName)
            {
                if (item.stackable)
                {
                    itemList.ToList()[i].uses += uses;
                    alreadyInInventory = true;
                    break;
                }
                else
                {
                    return;
                }
            }
                }
        if (!alreadyInInventory&&itemList.Count<inventorySize)
        {
            itemList.Enqueue(item);
        }

    }

    /// <summary>
    /// activates the item in the first inventory slot. if uses are 0, it is also removed;
    /// </summary>
    public void ActivateItem()
    {
        itemList.Peek().UseItem();
        if (itemList.Peek().uses == 0)
        {
            itemList.Dequeue();
        }
    }
    /// <summary>
    /// Puts the first item you have in the last slot;
    /// </summary>
    public void SwapItem()
    {
        itemList.Enqueue(itemList.Dequeue());
    }
}
