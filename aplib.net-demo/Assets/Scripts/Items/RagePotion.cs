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
        [SerializeField]
        private GameObject _rageEffect;

        [SerializeField]
        private int _damageIncreasePercentage = 50;

        private int _damageIncrease;

        [SerializeField]
        private float _duration = 3; // In seconds

        private GameObject _player;
        private Weapon _playerWeapon;

        protected override void Awake()
        {
            base.Awake();
            _player = GameObject.FindGameObjectWithTag("Player");
            _playerWeapon = _player.GetComponentInChildren<Weapon>();
            _damageIncrease = _playerWeapon.Damage * _damageIncreasePercentage / 100;
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
            _playerWeapon.Damage += _damageIncrease;

            // Add player visual effect as child of player
            GameObject rageEffect = Instantiate(_rageEffect, _player.transform);

            yield return new WaitForSeconds(_duration);

            _playerWeapon.Damage -= _damageIncrease;

            // Remove player visual effect
            Destroy(rageEffect);
        }
    }
}
