using System.Collections;
using UnityEngine;

namespace Teleporter
{
    /// <summary>
    /// An isolated teleporter set up to teleport the player to another target teleporter.
    /// Optionally, this target teleporter targets this teleporter back, coupling both.
    /// </summary>
    public class Teleporter : MonoBehaviour
    {
        /// <summary>
        /// The duration of the delay before the player gets teleported.
        /// </summary>
        [Header("Parameters")]
        public float teleportWindUpTime = 2f;

        /// <summary>
        /// An offset from this teleporter's position, on which the player will be teleported to when teleporting to this
        /// teleporter.
        /// </summary>
        public Vector3 landingPointOffset = new(0, 1, 0);

        /// <summary>
        /// A reference to the FX parent gameobject, containing all special effects of the teleporter to be
        /// activated during wind up.
        /// </summary>
        [Header("References")]
        public GameObject FX;
        
        /// <summary>
        /// The teleporter to which the player must be teleported. This is a one-directional link. To be bidirectional,
        /// the target teleported must target this teleporter as back.
        /// </summary>
        public Teleporter targetTeleporter;

        /// <summary>
        /// The absolute position to which the player will be teleported to when teleporting to this teleporter.
        /// </summary>
        protected Vector3 _landingPoint => transform.position + landingPointOffset;

        /// <summary>
        /// A simple reference to the player's transform, for performance reasons.
        /// </summary>
        protected Transform _playerTransform;

        /// <summary>
        /// A reference to the coroutine running while teleporting. Stored, such that the teleporting can be cancelled.
        /// </summary>
        private Coroutine _waitThenTeleportCoroutine;
        
        /// <summary>
        /// When the player gets teleported to this teleporter, you do not want logic to trigger which would normally
        /// be triggered when the player walks into the teleporter. For example, the player should not trigger this
        /// teleporter when being teleported to this teleporter.
        /// </summary>
        private bool _shouldIgnoreNextPlayerEntryTrigger;

        /// <summary>
        /// Get global GameObjet references once.
        /// </summary>
        private void Start()
        {
            _playerTransform = GameObject.FindWithTag("Player").transform;
        }

        /// <summary>
        /// When the player walks in, the teleporter should start winding up and teleport.
        /// </summary>
        /// <param name="other">The object which should be the player</param>
        private void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag("Player")) return; // Only trigger for player
            
            if (_shouldIgnoreNextPlayerEntryTrigger) 
            {
                // Do not teleport when the player triggers this because of being teleported to this teleporter
                _shouldIgnoreNextPlayerEntryTrigger = false;
                return;
            }

            FX.SetActive(true);
            _waitThenTeleportCoroutine = StartCoroutine(WaitThenTeleport()); // Store reference to be able to stop it prematurely
        }

        /// <summary>
        /// Triggered when the player exits prematurely to cancel the teleport, AND when the teleport was successful
        /// (as the player thus also exits it).
        /// </summary>
        /// <param name="other">The object which should be the player</param>
        private void OnTriggerExit(Collider other)
        {
            if (!other.CompareTag("Player")) return; // Only trigger for player

            FX.SetActive(false);
            if (_waitThenTeleportCoroutine != null) // Can be null when player gets teleported to this teleporter
                StopCoroutine(_waitThenTeleportCoroutine); // Stop coroutine prematurely when player exits during wind up time
        }

        /// <summary>
        /// Waits for <see cref="teleportWindUpTime"/>, then starts the teleport. This allows the FX to play.
        /// </summary>
        /// <returns>IEnumerator representing coroutine</returns>
        private IEnumerator WaitThenTeleport()
        {
            yield return new WaitForSeconds(teleportWindUpTime);
            
            // Teleport the player
            targetTeleporter.TeleportReceive(_playerTransform);
        }

        /// <summary>
        /// Teleport a <see cref="Transform"/> (normally the player) towards this teleporter.
        /// </summary>
        /// <param name="objectTransform">The transform to teleport</param>
        public void TeleportReceive(Transform objectTransform)
        {
            objectTransform.position = _landingPoint;
            // Prevent this action from triggering logic which are intended for when the player walks in.
            _shouldIgnoreNextPlayerEntryTrigger = true;
        }

        /// <summary>
        /// Draw visual aid in the unity editor, when this teleporter is selected.
        /// </summary>
        private void OnDrawGizmosSelected()
        {
            Gizmos.DrawWireSphere(_landingPoint, 0.2f); // Indicate own landingPoint
            Gizmos.DrawSphere(targetTeleporter._landingPoint, 0.3f); // Indicate target landingPoint
            Gizmos.DrawLine(_landingPoint, targetTeleporter._landingPoint); // Indicate which portals is targeted
        }
    }
}
