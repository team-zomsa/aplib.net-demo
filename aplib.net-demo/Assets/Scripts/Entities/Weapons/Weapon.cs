using UnityEngine;

/// <summary>
/// Abstract weapons class as a base for all weapons.
/// </summary>
public abstract class Weapon : MonoBehaviour
{
    /// <summary>
    /// The tag of the target that the weapon can hit.
    /// </summary>
    [SerializeField] protected string _targetTag = "Enemy";

    /// <summary>
    /// The amount of damage the weapon deals.
    /// </summary>
    [SerializeField] protected int _damage = 25;

    /// <summary>
    /// Initialize the weapon with the damage and target tag.
    /// </summary>
    protected void Initialize(int damage, string targetTag)
    {
        _damage = damage;
        _targetTag = targetTag;
    }

    /// <summary>
    /// Use the weapon.
    /// </summary>
    public abstract void UseWeapon();
}
