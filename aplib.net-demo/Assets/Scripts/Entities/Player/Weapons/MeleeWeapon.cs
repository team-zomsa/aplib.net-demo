using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// A basic melee weapon that deals damage to enemies within a short range.
/// The hitzone is a capsule shape defined by two spheres.
/// </summary>
public class MeleeWeapon : Weapon
{
    [SerializeField] private int _damage = 25;

    /// <summary>
    /// The height of the hitzone in world units.
    /// Could also be called length, but height is what Unity uses for capsules.
    /// </summary>
    [SerializeField] private float _height = 4;

    /// <summary>
    /// The radius of the hitzone in world units.
    /// The two spheres that define the hitzone have the same radius.
    /// </summary>
    [SerializeField] private float _radius = 1.4f;

    private Vector3 _sphere1Center;
    private Vector3 _sphere2Center;

    /// <summary>
    /// Ensure the height is at least twice the radius, because the height of the hitzone (capsule) must at least be the diameter of the spheres.
    /// (If the two spheres are at the same position, the capsule just becomes a sphere and height = 2 * radius)
    /// </summary>
    private void Start()
    {
        if (_height < 2 * _radius) _height = 2 * _radius;
        UpdateHitZone();
    }

    /// <summary>
    /// Update the HitZone Capsule defined by the locations of the two spheres.
    /// </summary>
    private void UpdateHitZone()
    {
        _sphere1Center = transform.position + _radius * transform.forward;
        _sphere2Center = transform.position + (_height - _radius) * transform.forward;
    }

    /// <summary>
    /// Check in the HitZone for any enemies and deal damage to them.
    /// </summary>
    public override void UseWeapon()
    {
        foreach (Collider collider in GetHitZone())
        {
            // Check if the collider with enemy tag has a Health component. If so, deal damage to it. 
            HealthComponent enemy = collider.GetComponent<HealthComponent>();
            enemy?.ReduceHealth(_damage);
        }
    }

    /// <summary>
    /// Get the targets currently in the hitzone
    /// </summary>
    /// <returns>A collection of targets</returns>
    public IEnumerable<Collider> GetHitZone()
    {
        UpdateHitZone();
        return Physics.OverlapCapsule(_sphere1Center, _sphere2Center, _radius)
            .Where(c => c.CompareTag(_targetTag));
    }

    /// <summary>
    /// Draw the hitzone when the weapon is selected in the editor.
    /// </summary>
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Globals.s_PhysicsColor;
        Gizmos.DrawWireSphere(_sphere1Center, _radius);
        Gizmos.DrawWireSphere(_sphere2Center, _radius);
    }
}
