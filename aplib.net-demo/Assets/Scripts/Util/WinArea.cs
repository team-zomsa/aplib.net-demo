using UnityEngine;

/// <summary>
/// The win area for the player.
/// The size is defined by the local scale of the transform.
/// </summary>
public class WinArea : Area
{
    private Inventory _inventory;

    public void Start()
    {
        GameObject inventoryObject = GameObject.Find("InventoryObject");

        if (inventoryObject == null) throw new UnityException("InventoryObject not found!");

        if (!inventoryObject.TryGetComponent(out _inventory)) throw new UnityException("Inventory not found!");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player") || !_inventory.ContainsItem("The Eternal Elixir")) return;

        Debug.Log("You win!");
        Application.Quit();
    }
}
