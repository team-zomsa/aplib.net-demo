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
        [SerializeField] private int _damage = 50;
        [SerializeField] private int _range = 50;

        /// <summary>
        /// Shoots a ray from the players's position in the direction it is facing.
        /// Look for an entity with an Enemy tag within range and deal damage to it.
        /// Crossbow can hit mutiple enemies IF they are on the same line.
        /// </summary>
        public override void UseWeapon()
        {
            RaycastHit[] hits = Physics.RaycastAll(Camera.main.transform.position, Camera.main.transform.forward, _range);

            // Grabs all objects that collide with this line by order of closest to the player till the furthest the range will let it be.
            IEnumerable<RaycastHit> orderedHits = hits.OrderBy(hit => hit.distance);

            // Will damage only enemies and the ray will stop when it hits an object that is not an enemy.
            foreach (RaycastHit hit in orderedHits)
            {
                if (!hit.collider.CompareTag("Enemy"))
                    break;

                // Check if the enemy has a Health component.
                HealthComponent enemy = hit.collider.GetComponent<HealthComponent>();
                enemy?.ReduceHealth(_damage);
            }
        }
    }
}
