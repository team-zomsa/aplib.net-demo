using Assets.Scripts.Items;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(AmmoPouch))]
[RequireComponent(typeof(KeyRing))]
public class Inventory : MonoBehaviour
{
    /// <summary>
    /// The texture of the _inventoryIndicator object.
    /// </summary>
    public Texture emptyInventoryImage;

    /// <summary>
    /// The canvas object that holds the inventory.
    /// </summary>
    public GameObject inventoryObject;

    [SerializeField] private float _inventorySize = 4;

    private AmmoPouch _ammoPouch;

    /// <summary>
    /// The RawImage is the object on which the _inventoryIndicator texture is projected.
    /// </summary>
    private RawImage _inventoryIndicator;

    private Queue<Item> _itemList;

    /// <summary>
    /// Creates the inventory queue and sets default size, resets the items, and fetches the rawimage component to display the
    /// icons.
    /// </summary>
    private void Start()
    {
        _ammoPouch = GetComponent<AmmoPouch>();
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
    public void PickUpItem(Item item)
    {
        if (item is AmmoItem)
        {
            _ammoPouch.AddAmmo(1);
            return;
        }

        if (!item.stackable)
        {
            // If the the item is non stackable, we cannot store it in the inventory system.
            return;
        }

        Item existingItem = _itemList.FirstOrDefault(i => i.name == item.name);
        bool alreadyInInventory = existingItem is not null;

        if (alreadyInInventory)
        {
            existingItem.uses += item.usesAddedPerPickup;
        }
        else if (_itemList.Count < _inventorySize)
        {
            item.Reset();

            // Make a copy of the item to add to the inventory, so that the 'real world' item can be destroyed.
            item.transform.SetParent(transform);
            Item itemCopy = Instantiate(item);
            itemCopy.gameObject.GetComponent<Collider>().enabled = false;
            itemCopy.transform.Find("Visual").gameObject.SetActive(false);
            itemCopy.name = item.name;
            _itemList.Enqueue(itemCopy);

            DisplayItem();
        }
    }

    /// <summary>
    /// Activates the item in the first inventory slot. If the item is depleted, it is removed from the inventory and a new
    /// item is selected.
    /// </summary>
    public void ActivateItem()
    {
        if (_itemList.Any())
        {
            _itemList.Peek().UseItem();

            if (_itemList.Peek().uses <= 0)
            {
                _ = _itemList.Dequeue();
            }
        }

        DisplayItem();
    }

    /// <summary>
    /// Puts the first item you have in the last slot.
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
    /// Fetches the _inventoryIndicator of the first item in the queue and makes it the texture of the displayed image.
    /// </summary>
    public void DisplayItem() => _inventoryIndicator.texture =
        _itemList.Count == 0 ? emptyInventoryImage : _itemList.Peek().iconTexture;
}
