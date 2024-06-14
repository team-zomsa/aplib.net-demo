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

        /// <summary>
        /// The duration of the rage potion effect in seconds.
        /// </summary>
        [field: SerializeField]
        public float Duration { get; private set; } = 3f;

        /// <summary>
        /// The visual effect that plays when the player uses the rage potion.
        /// </summary>
        [SerializeField]
        private GameObject _rageEffect;

        /// <summary>
        /// The concrete damage increase that the player receives from the rage potion.
        /// </summary>
        private int _damageIncrease;

        /// <summary>
        /// The player object in the scene.
        /// </summary>
        private GameObject _player;

        /// <summary>
        /// The player's equipment inventory.
        /// </summary>
        private EquipmentInventory _playerInventory;

        protected override void Awake()
        {
            base.Awake();
            _player = GameObject.FindGameObjectWithTag("Player");
            _playerInventory = _player.GetComponentInChildren<EquipmentInventory>();
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
            if (activeWeapon == null)
            {
                Debug.LogWarning("Player does not have a weapon equipped!.");
                yield break;
            }

            _damageIncrease = activeWeapon.Damage * DamageIncreasePercentage / 100;
            activeWeapon.Damage += _damageIncrease;

            // Add player visual effect as child of player
            GameObject rageEffect = Instantiate(_rageEffect, _player.transform);

            yield return new WaitForSeconds(Duration);

            activeWeapon.Damage -= _damageIncrease;

            // Remove player visual effect
            Destroy(rageEffect);
        }
    }
}
