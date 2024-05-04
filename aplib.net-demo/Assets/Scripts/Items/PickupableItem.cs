using UnityEngine;

namespace Assets.Scripts.Items
{
    /// <summary>
    /// Attach this class to make object pickupable.
    /// </summary>
    [RequireComponent(typeof(Item))]
    public class PickupableItem : MonoBehaviour
    {
        private void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag("Player"))
                return;

            GameObject inventoryObject = GameObject.Find("InventoryObject");
            Inventory inventory = inventoryObject.GetComponent<Inventory>();

            if (!inventory)
            {
                Debug.LogWarning("Inventory not found!");
                return;
            }

            Item item = gameObject.GetComponent<Item>();

            inventory.PickUpItem(item);
            Destroy(gameObject);
        }
    }
}
