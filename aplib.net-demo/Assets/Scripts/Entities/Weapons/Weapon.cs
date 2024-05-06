using UnityEngine;

/// <summary>
/// Abstract weapons class as a base for all weapons.
/// </summary>
public abstract class Weapon : MonoBehaviour
{
    // The player sound component used to play weapon sounds.
    protected PlayerSound _playerSound;

    /// <summary>
    /// The tag of the target that the weapon can hit.
    /// </summary>
    public string TargetTag = "Enemy";

    /// <summary>
    /// Get the PlayerSound component from the root object.
    /// </summary>
    protected void Start()
        => _playerSound = transform.root.GetComponent<PlayerSound>();

    /// <summary>
    /// Use the weapon.
    /// </summary>
    public abstract void UseWeapon();
}
