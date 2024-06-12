using Entities.Weapons;
using System.Collections;
using UnityEngine;

namespace Assets.Scripts.Items
{
    /// <summary>
    /// Rage potion that temporarily increases the player's damage.
    /// </summary>
    [RequireComponent(typeof(PickupableItem))]
    public class RagePotion : Item
    {
        /// <summary>
        /// The percentage increase of the player's damage.
        /// </summary>
        [field: SerializeField]
        public int DamageIncreasePercentage { get; private set; } = 100;

        [SerializeField]
        private GameObject _rageEffect;

        private int _damageIncrease;

        [SerializeField]
        private float _duration = 3; // In seconds

        private GameObject _player;
        private EquipmentInventory _playerInventory;

        protected override void Awake()
        {
            base.Awake();
            _player = GameObject.FindGameObjectWithTag("Player");
            EquipmentInventory _playerInventory = _player.GetComponentInChildren<EquipmentInventory>();
            Weapon activeWeapon = _playerInventory.CurrentEquipment as Weapon;
            _damageIncrease = activeWeapon.Damage * DamageIncreasePercentage / 100;
        }

        /// <summary>
        /// Uses the rage potion to temporarily increase the player's damage.
        /// </summary>
        public override void UseItem()
        {
            // Set object to active
            base.UseItem();
            StartCoroutine(ActivateRage());
        }

        private IEnumerator ActivateRage()
        {
            Weapon activeWeapon = _playerInventory.CurrentEquipment as Weapon;
            _damageIncrease = activeWeapon.Damage * DamageIncreasePercentage / 100;

            activeWeapon.Damage += _damageIncrease;

            // Add player visual effect as child of player
            GameObject rageEffect = Instantiate(_rageEffect, _player.transform);

            yield return new WaitForSeconds(_duration);

            activeWeapon.Damage -= _damageIncrease;

            // Remove player visual effect
            Destroy(rageEffect);
        }
    }
}
