using UnityEngine;

namespace Assets.Scripts.Items
{
    /// <summary>
    /// Attach this class to make object pickupable.
    /// </summary>
    [RequireComponent(typeof(Key))]
    public class PickupableKey : MonoBehaviour
    {
        private KeyRing _keyRing;

        public void Start()
        {
            GameObject inventoryObject = GameObject.Find("KeyRingObject");
            _keyRing = inventoryObject.GetComponent<KeyRing>();

            if (!_keyRing) throw new UnityException("Key ring not found!");
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag("Player"))
                return;

            Key item = gameObject.GetComponent<Key>();
            PointsManager.Instance.AddPoints(item);

            _keyRing.StoreKey(item);

            Destroy(gameObject);
        }
    }
}
