using UnityEngine;

namespace Entities.Weapons
{
    /// <summary>
    /// Abstract weapons class as a base for all weapons.
    /// </summary>
    public abstract class Weapon : Equipment
    {
        /// <summary>
        /// The entity sound component used to play weapon sounds.
        /// </summary>
        [SerializeField]
        protected EntitySound _entitySound;

        /// <summary>
        /// The tag of the target that the weapon can hit.
        /// </summary>
        [SerializeField]
        protected string _targetTag = "Enemy";

        /// <summary>
        /// The amount of damage the weapon deals.
        /// </summary>
        [field: SerializeField]
        public int Damage { get; set; } = 25;

        /// <summary>
        /// Initialize the weapon with the damage and target tag.
        /// </summary>
        protected void Initialize(int damage, string targetTag)
        {
            Damage = damage;
            _targetTag = targetTag;
            _entitySound = GetComponentInParent<EntitySound>();
        }

        public override void UseEquipment() => UseWeapon();

        /// <summary>
        /// Use the weapon.
        /// </summary>
        public abstract void UseWeapon();

        /// <summary>
        /// Check if the weapon is currently able to be activated, and is thus able to be animated.
        /// </summary>
        public virtual bool CanAnimate() => true;
    }
}
