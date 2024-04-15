using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class Inventory : MonoBehaviour
{

    Queue<Item> _itemList;
    public float inventorySize;
    /// <summary>
    /// The RawImage is the object on which the icon texture is projected
    /// </summary>
    public RawImage icon;
    /// <summary>
    /// the texture of the icon object
    /// </summary>
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
    }

    /// <summary>
    /// Converts queue to list to check if there are any items with matching names. If there are it checks if they are stackable and adds uses. If they are not it does nothing. If there are not matching names it adds the item to the inventory;
    /// </summary>
    /// <param name="item">The item that is fed into the inventory</param>
    /// <param name="uses">the amount of uses that are added upon pickup</param>
    public void PickUpItem(Item item, float uses = 1)
    {
        float queueSize = _itemList.Count();
        bool alreadyInInventory = false;
        for (int i = 0; i < queueSize; i++)
        {
            if (_itemList.ToList()[i].name == item.name)
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
        if (!alreadyInInventory && _itemList.Count < inventorySize)
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

        if (_itemList.Count() > 0)
        {
            _itemList.Peek().UseItem();

            if (_itemList.Peek().uses == 0)
            {
                _itemList.Peek().Reset();
                _itemList.Dequeue();
            }
        }
        DisplayItem();

    }

    /// <summary>
    /// Puts the first item you have in the last slot;
    /// </summary>
    public void SwitchItem()
    {
        if (_itemList.Count > 0)
        {
            _itemList.Enqueue(_itemList.Dequeue());
            DisplayItem();
        }
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
