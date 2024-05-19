using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;

public class Inventory : AbstractInventory
{
    protected Queue<Item> _itemList;
    public float inventorySize;

    /// <summary>
    /// Creates the inventory queue and sets default size, resets the items, and fetches the rawimage component to display the icons.
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
    /// If there are not matching names it adds the item to the inventory.
    /// </summary>
    /// <param name="item">The item that is fed into the inventory.</param>
    public override void PickUpItem(Item item)
    {
        if (item is not Weapon)
        {
            bool alreadyInInventory = false;
            List<Item> _tempItemList = _itemList.ToList();
            for (int i = 0; i < _itemList.Count; i++)
            {
                if (_tempItemList[i].name == item.name)
                {
                    if (!item.stackable)
                        return;

                    _tempItemList[i].uses += item.startUses;
                    alreadyInInventory = true;
                    break;
                }
            }

            if (!alreadyInInventory && _itemList.Count < inventorySize)
            {
                _itemList.Enqueue(item);
                DisplayItem();
            }
        }
    }

    /// <summary>
    /// Activates the item in the first inventory slot. If the item is depleted, it is removed from the inventory and a new item is selected.
    /// </summary>
    public override void ActivateItem()
    {
        if (_itemList.Any())
        {
            _itemList.Peek().UseItem();

            if (_itemList.Peek().uses == 0)
                _ = _itemList.Dequeue();
        }

        DisplayItem();
    }

    /// <summary>
    /// Puts the first item you have in the last slot.
    /// </summary>
    public override void SwitchItem()
    {
        if (_itemList.Any())
        {
            _itemList.Enqueue(_itemList.Dequeue());
            DisplayItem();
        }
    }

    /// <summary>
    /// Fetches the _inventoryIndicator of the first item in the queue and makes it the texture of the displayed image.
    /// </summary>
    public override void DisplayItem() => _inventoryIndicator.texture = _itemList.Count == 0 ? emptyInventoryImage : _itemList.Peek().iconTexture;
}
