using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    //TODO Add visual indicators
    //TODO Add max inventory size
    //TODO Add controls for using items
    //TODO Add controls for switching which item you're using
    //TODO Add way to get rid of items
    Queue<Item> itemList;
    // Start is called before the first frame update
    void Start()
    {
        itemList = new Queue<Item>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    /// <summary>
    /// Picks up an item and adds it to the inventory queue, if the item is already in the inventory, it adds uses instead
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
                itemList.ToList()[i].uses += uses;
                alreadyInInventory = true;
                break;
            }
                }
        if (!alreadyInInventory)
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
}
