using System;
using UnityEngine;

/// <summary>
/// Attach this class to make object pickable.
/// </summary>
public class PickupableItem : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        if (!collision.collider.CompareTag("Player"))
            return;

        GameObject inventoryObject = GameObject.Find("InventoryObject");
        Inventory inventory = inventoryObject.GetComponent<Inventory>();

        if (!inventory)
            throw new Exception("Inventory not found!");

        Item item = gameObject.GetComponent<Item>();

        if (!item)
            throw new Exception("Item not found!");

        inventory.PickUpItem(item);
        Destroy(gameObject);
    }
}
