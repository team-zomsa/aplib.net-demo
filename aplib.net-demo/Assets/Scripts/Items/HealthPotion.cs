using UnityEngine;

namespace Assets.Scripts.Items
{
    /// <summary>
    /// Health potion item that can be used to heal the player.
    /// </summary>
    [RequireComponent(typeof(PickupableItem))]
    public class HealthPotion : Item
    {
        [SerializeField]
        private GameObject _playerEffect;

        [SerializeField]
        private int _healAmount = 50;

        private HealthComponent _playerHealth;

        protected override void Awake()
        {
            base.Awake();
            _playerHealth = GameObject.FindGameObjectWithTag("Player").GetComponent<HealthComponent>();
        }

        /// <summary>
        /// Uses the health potion to heal the player.
        /// </summary>
        public override void UseItem()
        {
            base.UseItem();
            _playerHealth.IncreaseHealth(_healAmount);
            Instantiate(_playerEffect, _playerHealth.transform);
        }
    }
}
