using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Entities.Weapons
{
    /// <summary>
    /// This class has functions for ranged weapons.
    /// A bullet shoots through multiple enemies when they are on the same line in 3D space.
    /// The ranged weapon has a range.
    /// </summary>
    public class RangedWeapon : Weapon
    {
        /// <summary>
        /// The point from which the weapon will be fired.
        /// </summary>
        [SerializeField]
        private Transform _firePoint;

        [SerializeField]
        private int _range = 50;

        [SerializeField]
        private AmmoPouch _ammoPouch;

        private IEnumerable<RaycastHit> _orderedHits;

        /// <summary>
        /// By default, assume the weapon will be fired by the player, from the camera.
        /// </summary>
        private void Awake()
        {
            // This needs to be '==' and not 'is', because the check for None (transform) only works with '=='.
            if (_firePoint == null)
            {
                _firePoint = Camera.main.transform;
            }


            if (_ammoPouch == null)
            {
                Debug.LogError("AmmoPouch not assigned. Defaulting to parent ammo pouch.");
                _ammoPouch = GetComponentInParent<AmmoPouch>();

                if (_ammoPouch == null)
                {
                    Debug.LogError("No parent ammo pouch found. Disabling ranged weapon.");
                    enabled = false;
                }
            }
        }

        /// <summary>
        /// Initialize the weapon with the fire point, damage, range and target tag.
        /// </summary>
        /// <param name="firePoint">The point from which the weapon will be fired.</param>
        /// <param name="damage">The amount of damage the weapon will deal.</param>
        /// <param name="range">The range of the weapon.</param>
        /// <param name="targetTag">The tag of the target.</param>
        public void Initialize(int damage, string targetTag, Transform firePoint, int range)
        {
            Initialize(damage, targetTag);
            _firePoint = firePoint;
            _range = range;
        }

        /// <summary>
        /// Shoots a ray from the players's position in the direction it is facing.
        /// Look for an entity with an Enemy tag within range and deal damage to it.
        /// Crossbow can hit mutiple enemies IF they are on the same line.
        /// </summary>
        public override void UseWeapon()
        {
            if (!_ammoPouch.TryUseAmmo())
            {
                return;
            }

            // Play a random whoosh crossbow sound.
            _entitySound.Shoot();

            if (!EnemiesInLineOfSight())
            {
                return;
            }

            // Will damage only enemies and the ray will stop when it hits an object that is not an enemy.
            foreach (RaycastHit hit in _orderedHits)
            {
                // Stop when the ray hits an object that is not an enemy
                if (!hit.collider.CompareTag(_targetTag))
                    break;

                // Check if the enemy has a Health component.
                HealthComponent enemy = hit.collider.GetComponent<HealthComponent>();
                enemy?.ReduceHealth(Damage);
            }
        }

        /// <summary>
        /// Shoot a ray from the firepoint in the direction it is facing.
        /// Check only if the first object hit is an enemy.
        /// </summary>
        /// <returns>True if the first object hit is an enemy.</returns>
        public bool EnemiesInLineOfSight()
        {
            // Fire a debug ray in the line of sight
            Debug.DrawRay(_firePoint.position, _firePoint.transform.forward * _range, Globals.s_AimingColor);

            RaycastHit[] hits = Physics.RaycastAll(_firePoint.position, _firePoint.transform.forward, _range);
            _orderedHits = hits.OrderBy(hit => hit.distance);
            RaycastHit firstHit = _orderedHits.FirstOrDefault();

            return firstHit.collider != null && firstHit.collider.CompareTag(_targetTag);
        }

        /// <summary>
        /// Check if the weapon has enough ammunition to be used and thus animated.
        /// </summary>
        public override bool CanFire() => !_ammoPouch.IsEmpty();
    }
}
