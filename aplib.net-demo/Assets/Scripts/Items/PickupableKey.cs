using System;
using UnityEngine;

namespace Assets.Scripts.Items
{
    /// <summary>
    /// Attach this class to make object pickupable.
    /// </summary>
    [RequireComponent(typeof(Key))]
    [RequireComponent(typeof(KeyPointsContributor))]
    public class PickupableKey : MonoBehaviour
    {
        public event Action<Key> KeyPickedUp;

        private KeyRing _keyRing;
        private Key _item;
        private KeyPointsContributor _keyPointsAdder;

        public void Start()
        {
            GameObject inventoryObject = GameObject.Find("KeyRingObject");
            _keyRing = inventoryObject.GetComponent<KeyRing>();

            if (!_keyRing) throw new UnityException("Key ring not found!");

            _item = gameObject.GetComponent<Key>();
            _keyPointsAdder = GetComponent<KeyPointsContributor>();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag("Player") || !other.material.name.Contains("PlayerPhysic"))
                return;

            KeyPickedUp?.Invoke(_item);

            _keyRing.StoreKey(_item);

            Destroy(gameObject);
        }
    }
}
