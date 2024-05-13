using UnityEngine;
using UnityEngine.Serialization;

/// <summary>
/// Class representing an Ammo Pouch.
/// </summary>
public class AmmoPouch : MonoBehaviour
{
    [FormerlySerializedAs("MaxAmmoCount")] [SerializeField]
    public int maxAmmoCount;

    private int _currentAmmoCount;

    private void Start() => _currentAmmoCount = maxAmmoCount;

    /// <summary>
    /// Adds ammunition to the ammo pouch.
    /// </summary>
    /// <param name="ammo">The amount of ammunition to add.</param>
    public void AddAmmo(int ammo)
    {
        Debug.Log("Add ammunition to pouch!");
        _currentAmmoCount += ammo;
        if (_currentAmmoCount > maxAmmoCount)
        {
            Debug.Log($"Too much ammo! ({_currentAmmoCount})");
            _currentAmmoCount = maxAmmoCount;
        }
    }

    /// <summary>
    /// Tries to use the ammunition from the ammo pouch.
    /// </summary>
    /// <remarks>If there is enough ammo, one ammunition will be used.</remarks>
    /// <returns>True if there is enough ammunition to use, false otherwise.</returns>
    public bool TryUseAmmo()
    {
        if (_currentAmmoCount == 0)
        {
            return false;
        }

        _currentAmmoCount--;
        return true;
    }
}
