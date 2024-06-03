using UnityEngine;

namespace Assets.Scripts.Items
{
    /// <summary>
    /// Attach this class to make object pickupable.
    /// </summary>
    [RequireComponent(typeof(Key))]
    [RequireComponent(typeof(PointsAdderComponent))]
    public class PickupableKey : MonoBehaviour
    {
        private KeyRing _keyRing;
        private Key _item;
        private PointsAdderComponent _pointsAdderComponent;

        public void Start()
        {
            GameObject inventoryObject = GameObject.Find("KeyRingObject");
            _keyRing = inventoryObject.GetComponent<KeyRing>();

            if (!_keyRing) throw new UnityException("Key ring not found!");

            _item = gameObject.GetComponent<Key>();
            _pointsAdderComponent = GetComponent<PointsAdderComponent>();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag("Player") || !other.material.name.Contains("PlayerPhysic"))
                return;

            _pointsAdderComponent.SendPoints();

            _keyRing.StoreKey(_item);

            Destroy(gameObject);
        }
    }
}
