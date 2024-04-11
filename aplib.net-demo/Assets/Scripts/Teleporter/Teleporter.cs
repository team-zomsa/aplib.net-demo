using System.Collections;
using UnityEngine;

namespace Teleporter
{
    public class Teleporter : MonoBehaviour
    {
        public float teleportDelay;
        
        public GameObject FX;
        public Teleporter targetTeleporter;
        public Vector3 landingPointOffset; // TODO draw gizmo en defaut value? En comment dit

        protected Vector3 _landingPoint => transform.position + landingPointOffset;
        
        private Coroutine _waitThenTeleportCoroutine;
        private bool _shouldIgnoreNextPlayerEntry;

        private void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag("Player")) return; // Only trigger for player
            if (_shouldIgnoreNextPlayerEntry)
            {
                Debug.Log("Ignored...", this);
                _shouldIgnoreNextPlayerEntry = false;
                return;
            }

            Debug.Log("Enter!");

            FX.SetActive(true);
            _waitThenTeleportCoroutine = StartCoroutine(WaitThenTeleport()); // Store reference to be able to stop it prematurely
        }

        /// <summary>
        /// Triggered when the player exits prematurely to cancel the teleport, AND when the teleport was successful
        /// (as the player thus also exits it)
        /// </summary>
        /// <param name="other">TODO</param>
        private void OnTriggerExit(Collider other)
        {
            if (!other.CompareTag("Player")) return; // Only trigger for player

            Debug.Log("Exit!");

            FX.SetActive(false);
            StopCoroutine(_waitThenTeleportCoroutine); // Stop coroutine prematurely if still active
        }

        private IEnumerator WaitThenTeleport()
        {
            yield return new WaitForSeconds(teleportDelay);
            
            // Teleport the player
            // TODO finding the player is repetitive code. Remove it
            Transform playerTransform = GameObject.FindWithTag("Player").transform;
            targetTeleporter.TeleportReceive(playerTransform);
        }

        public void TeleportReceive(Transform objectTransform)
        {
            objectTransform.position = _landingPoint;
            _shouldIgnoreNextPlayerEntry = true;
        }

        private void OnDrawGizmos()
        {
            Gizmos.DrawSphere(_landingPoint, 0.3f);
        }
    }
}
