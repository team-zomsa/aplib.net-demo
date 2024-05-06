using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// This class has fuctions for ranged weapons.
/// A bullet shoots through multiple enemies when they are on the same line in 3D space.
/// The ranged weapon has a range.
/// </summary>
public class RangedWeapon : Weapon
{
    /// <summary>
    /// The point from which the weapon will be fired.
    /// </summary>
    public Transform FirePoint;
    [SerializeField] private int _damage = 50;
    [SerializeField] private int _range = 50;
    private IEnumerable<RaycastHit> _orderedHits;

    /// <summary>
    /// By default, assume the weapon will be fired by the player, from the camera.
    /// </summary>
    private void Start()
    {
        if (FirePoint == null)
            FirePoint = Camera.main.transform;
    }

    /// <summary>
    /// Shoots a ray from the players's position in the direction it is facing.
    /// Look for an entity with an Enemy tag within range and deal damage to it.
    /// Crossbow can hit mutiple enemies IF they are on the same line.
    /// </summary>
    public override void UseWeapon()
    {
        if (!EnemiesInRange())
            return;

        // Will damage only enemies and the ray will stop when it hits an object that is not an enemy.
        foreach (RaycastHit hit in _orderedHits)
        {
            if (!hit.collider.CompareTag(TargetTag))
                break;

            // Check if the enemy has a Health component.
            HealthComponent enemy = hit.collider.GetComponent<HealthComponent>();
            enemy?.ReduceHealth(_damage);
        }
    }

    /// <summary>
    /// Shoot a ray from the firepoint in the direction it is facing.
    /// Check only if the first object hit is an enemy.
    /// </summary>
    /// <returns>True if the first object hit is an enemy.</returns>
    public bool EnemiesInRange()
    {
        RaycastHit[] hits = Physics.RaycastAll(FirePoint.position, FirePoint.transform.forward, _range);
        _orderedHits = hits.OrderBy(hit => hit.distance);
        RaycastHit firstHit = _orderedHits.FirstOrDefault();

        return firstHit.collider != null && firstHit.collider.CompareTag(TargetTag);
    }
}
