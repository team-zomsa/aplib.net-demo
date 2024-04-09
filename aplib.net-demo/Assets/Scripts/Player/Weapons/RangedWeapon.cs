using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

/// <summary>
/// This class has fuctions for ranged weapons.
/// A bullet shoots through multiple enemies when they are on the same line in 3D space.
/// The ranged weapon has a range.
/// </summary>
public class RangedWeapon : Weapon
{
    [SerializeField] private int _damage = 30;
    [SerializeField] private int _range = 50;

    private Transform _playerTransform;


    // Set weapon transformation (pos and angle) at the player transform.
    void Start()
    {
        _playerTransform = transform.parent;
    }

    /// <summary>
    /// Shoots a ray from the players's position in the direction it is facing.
    /// Look for an entity with an Enemy tag within range and deal damage to it.
    /// Crossbow can hit mutiple enemies IF they are on the same line.
    /// </summary>
    public override void UseWeapon()
    {
        RaycastHit[] hits = Physics.RaycastAll(CameraManager.Instance.Camera.transform.position, CameraManager.Instance.Camera.transform.forward, _range);

        IEnumerable<RaycastHit> orderedHits = hits.OrderBy(hit => hit.distance);

        foreach (RaycastHit hit in orderedHits)
        {
            if (!hit.collider.CompareTag("Enemy"))
                break;

            // We need to check if the enemy has an BasicEnemy component before dealing damage
            BasicEnemy enemy = hit.collider.GetComponent<BasicEnemy>() ?? hit.collider.GetComponentInParent<DummyEnemy>();
            if (enemy != null) enemy.TakeDamage(_damage);
        }
    }
}
