using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Abstract weapons class as a base for all weapons.
/// </summary>
public abstract class Weapon : Item
{
    public virtual void UseWeapon() { }

    [SerializeField]
    private RawImage _icon;
}
