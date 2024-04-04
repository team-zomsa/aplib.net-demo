using System.Linq;
using UnityEngine;

/// <summary>
/// A basic melee weapon that deals damage to enemies within a short range.
/// </summary>
public class MeleeWeapon : Weapon
{
    [SerializeField] private int _damage = 25;
    [SerializeField] private float _height = 4;
    [SerializeField] private float _radius = 1.4f;

    private Vector3 _sphere1Center;
    private Vector3 _sphere2Center;

    /// <summary>
    /// Set the player's visual transform instance and ensure the height is at least twice the radius.
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
        UpdateHitZone();
        foreach (Collider collider in Physics.OverlapCapsule(_sphere1Center, _sphere2Center, _radius)
            .Where(c => c.CompareTag("Enemy")))
        {
            // Check if the collider with enemy tag has a BasicEnemy component. If so, deal damage to it. 
            BasicEnemy enemy = collider.GetComponent<BasicEnemy>();
            enemy?.TakeDamage(_damage);
        }
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
