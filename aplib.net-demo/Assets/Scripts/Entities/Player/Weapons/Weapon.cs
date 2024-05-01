using UnityEngine;

/// <summary>
/// Abstract weapons class as a base for all weapons.
/// </summary>
public abstract class Weapon : MonoBehaviour
{
    [SerializeField] protected string _targetTag = "Enemy";
    public string TargetTag
    {
        get => _targetTag;
        set => _targetTag = value;
    }
    public virtual void UseWeapon() { }
}
