using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Entities.Weapons
{
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
        [SerializeField]
        private Transform _firePoint;

        [SerializeField]
        private int _range = 50;

        private IEnumerable<RaycastHit> _orderedHits;

        /// <summary>
        /// By default, assume the weapon will be fired by the player, from the camera.
        /// </summary>
        private void Awake()
        {
            if (_firePoint == null)
                _firePoint = Camera.main.transform;
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
            if (!EnemiesInLineOfSight())
                return;

            // Will damage only enemies and the ray will stop when it hits an object that is not an enemy.
            foreach (RaycastHit hit in _orderedHits)
            {
                if (!hit.collider.CompareTag(_targetTag))
                    break;

                // Check if the enemy has a Health component.
                HealthComponent enemy = hit.collider.GetComponent<HealthComponent>();
                enemy?.ReduceHealth(_damage);
            }

            // Play a random whoosh crossbow sound.
            _entitySound.Shoot();
        }

        /// <summary>
        /// Shoot a ray from the firepoint in the direction it is facing.
        /// Check only if the first object hit is an enemy.
        /// </summary>
        /// <returns>True if the first object hit is an enemy.</returns>
        public bool EnemiesInLineOfSight()
        {
            RaycastHit[] hits = Physics.RaycastAll(_firePoint.position, _firePoint.transform.forward, _range);
            _orderedHits = hits.OrderBy(hit => hit.distance);
            RaycastHit firstHit = _orderedHits.FirstOrDefault();

            return firstHit.collider != null && firstHit.collider.CompareTag(_targetTag);
        }
    }
}
