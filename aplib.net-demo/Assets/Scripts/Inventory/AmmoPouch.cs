using UnityEngine;
using UnityEngine.Serialization;

/// <summary>
/// Class representing an Ammo Pouch.
/// </summary>
public class AmmoPouch : MonoBehaviour
{
    /// <summary>
    /// The maximum amount of ammunition that can be stored in the ammo pouch.
    /// </summary>
    [FormerlySerializedAs("MaxAmmoCount")]
    [SerializeField]
    public int maxAmmoCount;

    public int CurrentAmmoCount { get; private set; }

    private void Start() => CurrentAmmoCount = maxAmmoCount;

    /// <summary>
    /// Adds ammunition to the ammo pouch.
    /// </summary>
    /// <param name="ammo">The amount of ammunition to add.</param>
    public void AddAmmo(int ammo)
    {
        CurrentAmmoCount += ammo;
        if (CurrentAmmoCount > maxAmmoCount)
        {
            CurrentAmmoCount = maxAmmoCount;
        }
    }

    /// <summary>
    /// Tries to use the ammunition from the ammo pouch.
    /// </summary>
    /// <remarks>If there is enough ammo, one ammunition will be used.</remarks>
    /// <returns>True if there is enough ammunition to use, false otherwise.</returns>
    public bool TryUseAmmo()
    {
        if (CurrentAmmoCount == 0) return false;

        CurrentAmmoCount--;
        return true;
    }

    /// <summary>
    /// Returns whether the ammo pouch is empty.
    /// The second check is inserted to align with the rest of the logic.
    /// </summary>
    /// <returns>True if the ammo pouch is empty, false otherwise.</returns>
    public bool IsEmpty() => CurrentAmmoCount <= 0 && maxAmmoCount >= 0;
}
