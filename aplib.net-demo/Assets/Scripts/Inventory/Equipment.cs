// This program has been developed by students from the bachelor Computer Science at Utrecht
// University within the Software Project course.
// Copyright Utrecht University (Department of Information and Computing Sciences)

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
