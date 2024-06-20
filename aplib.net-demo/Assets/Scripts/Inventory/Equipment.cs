using UnityEngine;

/// <summary>
/// Equipment item, which can be used by the player.
/// </summary>
public abstract class Equipment : MonoBehaviour
{
    /// <summary>
    /// Method for using the equipment.
    /// </summary>
    public abstract void UseEquipment();
}
