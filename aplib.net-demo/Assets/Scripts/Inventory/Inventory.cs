using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class Inventory : MonoBehaviour
{
    private Queue<Item> _itemList;
    public float inventorySize;
    /// <summary>
    /// The RawImage is the object on which the _inventoryIndicator texture is projected
    /// </summary>
    private RawImage _inventoryIndicator;
    /// <summary>
    /// the texture of the _inventoryIndicator object
    /// </summary>
    public Texture emptyInventoryImage;
    public GameObject inventoryObject;

    //here you would add all the possible items you can add, you add them to the inventory by calling PickUpItem method with the right item

    /// <summary>
    /// Creates the inventory queue and fetches the rawimage component to display the icons
    /// </summary>
    private void Start()
    {
        _inventoryIndicator = GetComponent<RawImage>();
        _itemList = new Queue<Item>();
        DisplayItem();
    }

    /// <summary>
    /// Converts queue to list to check if there are any items with matching names.
    /// If there are it checks if they are stackable and adds uses. If they are not it does nothing. 
    /// If there are not matching names it adds the item to the inventory;
    /// </summary>
    /// <param name="item">The item that is fed into the inventory</param>
    /// <param name="uses">the amount of uses that are added upon pickup</param>
    public void PickUpItem(Item item, float uses = 1)
    {
        float queueSize = _itemList.Count;
        bool alreadyInInventory = false;
        List<Item> _tempItemList = _itemList.ToList();
        for (int i = 0; i < queueSize; i++)
        {
            if (_tempItemList[i].name == item.name)
            {
                if (item.stackable)
                {
                    _tempItemList[i].uses += uses;
                    alreadyInInventory = true;
                    break;
                }
                else
                {
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
    /// activates the item in the first inventory slot. if uses are 0, it is also removed, then it changes the _inventoryIndicator to the next item in the queue
    /// </summary>
    public void ActivateItem()
    {

        if (_itemList.Any())
        {
            _itemList.Peek().UseItem();

            if (_itemList.Peek().uses == 0)
            {
                //since we reuse the items, upon dequeueing the item gets reset to its default uses, so that when it is picked up again, the amount of uses is correct
                _itemList.Peek().Reset();
                _ = _itemList.Dequeue();
            }
        }

        DisplayItem();

    }

    /// <summary>
    /// Puts the first item you have in the last slot;
    /// </summary>
    public void SwitchItem()
    {
        if (_itemList.Any())
        {
            _itemList.Enqueue(_itemList.Dequeue());
            DisplayItem();
        }
    }

    /// <summary>
    /// Fetches the _inventoryIndicator of the first item in the queue and makes it the texture of the displayed image
    /// </summary>
    public void DisplayItem() => _inventoryIndicator.texture = _itemList.Count == 0 ? emptyInventoryImage : _itemList.Peek().iconTexture;
}
