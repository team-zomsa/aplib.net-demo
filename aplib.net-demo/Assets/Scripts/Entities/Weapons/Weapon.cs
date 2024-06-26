// This program has been developed by students from the bachelor Computer Science at Utrecht
// University within the Software Project course.
// Copyright Utrecht University (Department of Information and Computing Sciences)

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
        /// The entity animator component used to play weapon animations.
        /// </summary>
        [SerializeField]
        protected Animator _entityAnimator;

        /// <summary>
        /// The attack animation name on the animator.
        /// </summary>
        [SerializeField]
        protected string _attackAnimationName = "PlayerAttack";

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
            _entityAnimator = GetComponentInParent<Animator>();
        }

        public override void UseEquipment()
        {
            UseWeapon();

            if (!CanFire()) return;
            if (_entityAnimator == null) return;
            if (_attackAnimationName == string.Empty) return;

            _entityAnimator.SetTrigger(_attackAnimationName);
        }

        /// <summary>
        /// Use the weapon.
        /// </summary>
        public abstract void UseWeapon();

        /// <summary>
        /// Check if the weapon is currently able to be activated, and is thus able to be animated.
        /// </summary>
        public virtual bool CanFire() => true;
    }
}
