using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class Inventory : MonoBehaviour
{
    private Queue<Item> _itemList;
    private List<Key> _keyRing;
    public float inventorySize;
    /// <summary>
    /// The RawImage is the object on which the _inventoryIndicator texture is projected.
    /// </summary>
    private RawImage _inventoryIndicator;
    /// <summary>
    /// The texture of the _inventoryIndicator object.
    /// </summary>
    public Texture emptyInventoryImage;
    public GameObject inventoryObject;

    /// <summary>
    /// Creates the inventory queue and sets default size, resets the items, and fetches the rawimage component to display the icons.
    /// </summary>
    private void Start()
    {
        _inventoryIndicator = GetComponent<RawImage>();
        _itemList = new Queue<Item>();
        _keyRing = new List<Key>();
    }

    /// <summary>
    /// Converts queue to list to check if there are any items with matching names. If there are it checks if they are stackable and adds uses. If they are not it does nothing. If there are not matching names it adds the item to the inventory.
    /// </summary>
    /// <param name="item">The item that is fed into the inventory</param>
    /// <param name="uses">The amount of uses that are added upon pickup.</param>
    public void PickUpItem(Item item, float uses = 1)
    {
        bool alreadyInInventory = false;
        List<Item> _tempItemList = _itemList.ToList();
        for (int i = 0; i < _itemList.Count; i++)
        {
            if (_tempItemList[i].name == item.name)
            {
                if (!item.stackable)
                    return;

                _tempItemList[i].uses += uses;
                alreadyInInventory = true;
                break;
            }
            else
            {
                DisplayItem();
                return;
            }
        }


        if (!alreadyInInventory && _itemList.Count < inventorySize)
        {
            _itemList.Enqueue(item);
            DisplayItem();
        }
    }
    /// <summary>
    /// Adds a key to the keychain.
    /// </summary>
    /// <param name="key">The key to be stored.</param>
    public void StoreKey(Key key) => _keyRing.Add(key);

    /// <summary>
    /// Checks the ID of all keys in the keyring against the inputted doorId, if a match is found, true is returned and the key is consumed.
    /// </summary>
    /// <param name="doorId">The Id of the inputted door that is then checked against all the keys in the keyring.</param>
    /// <returns></returns>
    public bool KeyQuery(int doorId)
    {
        foreach (Key k in _keyRing)
        {
            if (k.id == doorId)
            {
                _ = _keyRing.Remove(k);
                return true;
            }
        }

        return false;

    }

    /// <summary>
    /// Activates the item in the first inventory slot. if uses are 0, it is also removed, then it changes the icon to the next item in the queue.
    /// </summary>
    public void ActivateItem()
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
    public void DisplayItem() => _inventoryIndicator.texture = _itemList.Count == 0 ? emptyInventoryImage : _itemList.Peek().iconTexture;
}
