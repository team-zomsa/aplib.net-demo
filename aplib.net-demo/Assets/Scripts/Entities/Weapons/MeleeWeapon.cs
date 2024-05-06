using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Entities.Weapons
{
    /// <summary>
    /// A basic melee weapon that deals damage to enemies within a short range.
    /// The hitzone is a capsule shape defined by two spheres.
    /// </summary>
    public class MeleeWeapon : Weapon
    {
        /// <summary>
        /// The amount of damage the weapon deals.
        /// </summary>
        [SerializeField] public int Damage = 25;

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
        private IEnumerable<Collider> _targets;

        /// <summary>
        /// Ensure the height is at least twice the radius, because the height of the hitzone (capsule) must at least be the diameter of the spheres.
        /// (If the two spheres are at the same position, the capsule just becomes a sphere and height = 2 * radius)
        /// </summary>
        private void Awake()
        {
            if (_height < 2 * _radius) _height = 2 * _radius;
            EnemiesWithinRange();
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
            if (EnemiesWithinRange())
            {
                foreach (Collider collider in _targets)
                {
                    // Check if the collider with enemy tag has a Health component. If so, deal damage to it. 
                    HealthComponent enemy = collider.GetComponent<HealthComponent>();
                    enemy?.ReduceHealth(Damage);
                }
            }
        }

        /// <summary>
        /// Whether the weapon can hit any enemies in the hitzone.
        /// If the multiple colliders are part of the same object, count them as one.
        /// </summary>
        /// <returns>True if there are enemies in the hitzone, false otherwise.</returns>
        public bool EnemiesWithinRange()
        {
            UpdateHitZone();
            _targets = Physics.OverlapCapsule(_sphere1Center, _sphere2Center, _radius)
                .Where(c => c.CompareTag(TargetTag)).GroupBy(c => c.transform.root).Select(g => g.First());
            return _targets.Any();
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
}
