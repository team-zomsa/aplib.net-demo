using UnityEngine;

/// <summary>
/// Abstract weapons class as a base for all weapons.
/// </summary>
public abstract class Weapon : MonoBehaviour
{
    // The player sound component used to play weapon sounds.
    protected PlayerSound _playerSound;

    /// <summary>
    /// Get the PlayerSound component from the root object.
    /// </summary>
    protected void Start()
    {
        _playerSound = transform.root.GetComponent<PlayerSound>();
    }

    public virtual void UseWeapon() { }
}
