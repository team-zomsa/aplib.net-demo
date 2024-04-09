using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class Inventory : MonoBehaviour
{
    //TODO Add icon logic to Item class
    //TODO Add hotbar Object which contains 4 subjobjects. They are linked to the inventory class and will display the right icons depending on inventory contents.
    //TODO Add controls for using items to InputManager(?) class
    //TODO Add controls for switching which item you're using to InputManager(?) class
    //TODO Add logic for items with a duration and effect
    Queue<Item> _itemList;
    public float inventorySize;
    //The RawImage is the object on which the icon texture is projected
    public RawImage icon;
    public Texture iconTexture;
    public GameObject inventoryObject;
    //here you would add all the possible items you can add, you add them to the inventory by calling PickUpItem method with the right item
    public Item testItem;
    public Item testItem2;
    /// <summary>
    /// Creates the inventory queue and sets default size, resets the items, and fetches the rawimage component to display the icons
    /// </summary>
    void Start()
    {
        //since we reuse items, it helps to reset the values for each of them on startup to avoid rare bugs with item uses
        testItem.Reset();
        testItem2.Reset();

        icon = GetComponent<RawImage>();
        _itemList = new Queue<Item>();
        inventorySize = 4;
        TestItemAdd();
    }
    /// <summary>
    /// This is where you should test the items, see if you can add and activate some items using the PickUpItem(item) and ActivateItem() functions
    /// /// </summary>
    void TestItemAdd()
    {
        PickUpItem(testItem);
        PickUpItem(testItem2);/*
        ActivateItem();
        ActivateItem();*/
    }
    /// <summary>
    /// Converts queue to list to check if there are any items with matching names. If there are it checks if they are stackable and adds uses. If they are not it does nothing. If there are not matching names it adds the item to the inventory;
    /// </summary>
    /// <param name="item"></param>
    /// <param name="uses"></param>
    public void PickUpItem(Item item, float uses = 1)
    {
        float queueSize = _itemList.Count();
        bool alreadyInInventory=false;

            for (int i = 0; i < queueSize; i++) {
            if (_itemList.ToList()[i].itemName == item.itemName)
            {
                if (item.stackable)
                {
                    _itemList.ToList()[i].uses += uses;
                    alreadyInInventory = true;
                    break;
                }
                else
                {
                    DisplayItem();
                    return;
                }
            }
                }
        if (!alreadyInInventory&&_itemList.Count<inventorySize)
        {
            _itemList.Enqueue(item);
            DisplayItem();
        }

    }

    /// <summary>
    /// activates the item in the first inventory slot. if uses are 0, it is also removed, then it changes the icon to the next item in the queue
    /// </summary>
    public void ActivateItem()
    {
        if (_itemList.Count()> 0)
            {
            _itemList.Peek().UseItem();
            if (_itemList.Peek().uses == 0)
            {
                _itemList.Dequeue().Reset();
            }
            DisplayItem();
        }
    }
    /// <summary>
    /// Puts the first item you have in the last slot;
    /// </summary>
    public void SwitchItem()
    {
        _itemList.Enqueue(_itemList.Dequeue());
        DisplayItem();
    }
    /// <summary>
    /// Fetches the icon of the first item in the queue and makes it the texture of the displayed image
    /// </summary>
    public void DisplayItem()
    {
        if (_itemList.Count == 0)
        {
            icon.texture = iconTexture;
        }
        else
        {
            icon.texture = 
                _itemList.Peek().iconTexture;
        }
    }
}
