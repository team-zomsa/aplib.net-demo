using UnityEngine;

namespace Assets.Scripts.Items
{
    /// <summary>
    /// Attach this class to make object pickupable.
    /// </summary>
    [RequireComponent(typeof(Key))]
    public class PickupableKey : MonoBehaviour
    {
        private void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag("Player"))
                return;

            GameObject inventoryObject = GameObject.Find("KeyRingObject");
            KeyRing keyring = inventoryObject.GetComponent<KeyRing>();

            if (!keyring)
            {
                Debug.LogWarning("Key ring not found!");
                return;
            }

            Key item = gameObject.GetComponent<Key>();

            keyring.StoreKey(item);

            Destroy(gameObject);
        }
    }
}
