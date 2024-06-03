using System;
using UnityEngine;

/// <summary>
/// The win area for the player.
/// The size is defined by the local scale of the transform.
/// </summary>
[RequireComponent(typeof(BoxCollider))]
public class WinArea : Area
{
    public event Action OnWin;

    private Inventory _inventory;

    public void Start()
    {
        GameObject inventoryObject = GameObject.Find("InventoryObject");

        if (inventoryObject == null) throw new UnityException("InventoryObject not found!");

        if (!inventoryObject.TryGetComponent(out _inventory)) throw new UnityException("Inventory not found!");
    }

    private void OnTriggerEnter(Collider other)
    {
        // Check for material to prevent double triggering.
        if (!other.CompareTag("Player") || !_inventory.ContainsItem("The Eternal Elixir")
            || !other.material.name.Contains("PlayerPhysic")) return;

        OnWin.Invoke();
    }
}
