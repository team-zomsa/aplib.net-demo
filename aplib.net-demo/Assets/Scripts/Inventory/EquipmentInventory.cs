using Entities.Weapons;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

/// <summary>
/// Represents an inventory of equipment items.
/// </summary>
public class EquipmentInventory : MonoBehaviour
{
    /// <summary>
    /// The default equipment collection.
    /// </summary>
    /// <remarks>
    /// This collection contains the default equipment items that are added to the equipment inventory when it is instantiated.
    /// </remarks>
    public List<IEquipment> DefaultEquipment = new();

    private List<IEquipment> _equipmentList;

    [SerializeField]
    private int inventorySize = 2;

    private int _currentEquipmentIndex;

    /// <summary>
    /// Specifies whether to switch to the new item when it is equipped.
    /// </summary>
    /// <remarks>
    /// If <c>true</c>, the <see cref="CurrentEquipment"/> will be automatically updated to the newly equipped item.
    /// </remarks>
    /// <value>
    /// <c>true</c> if switching to the new item is enabled; otherwise, <c>false</c>.
    /// </value>
    /// <seealso cref="EquipmentInventory.EquipItem(IEquipment)"/>
    /// <seealso cref="EquipmentInventory.CurrentEquipment"/>
    [SerializeField] public bool SwitchToNewItem;

    /// <summary>
    /// Specifies whether the EquipmentInventory has any items.
    /// </summary>
    /// <remarks>
    /// This property returns <c>true</c> if the EquipmentInventory has at least one item in its EquipmentList, and <c>false</c> otherwise.
    /// </remarks>
    /// <value><c>true</c> if the EquipmentInventory has items; otherwise, <c>false</c>.</value>
    /// <seealso cref="EquipmentInventory.EquipItem(IEquipment)"/>
    public bool HasItems => _equipmentList?.Count > 0;

    /// <summary>
    /// Gets the currently equipped item from the equipment inventory.
    /// </summary>
    /// <value>
    /// The currently equipped item.
    /// </value>
    public IEquipment CurrentEquipment => _equipmentList[_currentEquipmentIndex];

    private void Start()
    {
        // If the default equipment is empty, add the default equipment to the equipment list.
        // This is to ensure backward compatibility with the previous implementation.
        if (DefaultEquipment?.Count == 0)
        {
            Debug.LogWarning(
                "Default equipment is empty! Rolling back to obsolete implementation. \n "
                + "Make sure to set the default equipment in the inspector."
            );
            GameObject playerObject = GameObject.FindGameObjectWithTag("Player");

            DefaultEquipment = new(playerObject.transform.GetComponentsInChildren<Weapon>());
        }

        _equipmentList = new List<IEquipment>();
        _equipmentList.AddRange(DefaultEquipment);
    }

    /// <summary>
    /// Equip the specified equipment item to the EquipmentInventory.
    /// </summary>
    /// <param name="equipment">The equipment item to be equipped.</param>
    public void EquipItem(IEquipment equipment)
    {
        if (_equipmentList.Count >= inventorySize)
        {
            Debug.LogWarning("Inventory is full!");
            return;
        }

        if (_equipmentList.Contains(equipment))
        {
            Debug.LogWarning("Item already in inventory!");
            return;
        }

        _equipmentList.Add(equipment);

        if (SwitchToNewItem)
        {
            _currentEquipmentIndex = _equipmentList.Count - 1;
        }
    }

    /// <summary>
    /// Switches the current equipment item in the EquipmentInventory to the item at the specified index.
    /// </summary>
    /// <param name="index">The index of the equipment item to switch to.</param>
    public void SwitchEquipment(int index)
    {
        if (index < 0 || index >= _equipmentList.Count)
        {
            Debug.LogWarning("Index out of range!");
            return;
        }

        _currentEquipmentIndex = index;
    }

    /// <summary>
    /// Moves to the next equipment item in the EquipmentList.
    /// </summary>
    public void MoveNext()
    {
        int newIndex = (_currentEquipmentIndex + 1) % _equipmentList.Count;
        SwitchEquipment(newIndex);
    }

    /// <summary>
    /// Moves to the previous equipment item in the EquipmentList.
    /// </summary>
    public void MovePrevious()
    {
        int newIndex = (_currentEquipmentIndex - 1 + _equipmentList.Count) % _equipmentList.Count;
        SwitchEquipment(newIndex);
    }
}
