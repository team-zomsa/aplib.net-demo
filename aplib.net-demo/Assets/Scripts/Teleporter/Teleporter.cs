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
        [SerializeField]
        private float _teleportWindUpTime = 2f;

        /// <summary>
        /// A reference to the FX parent gameobject, containing all special effects of the teleporter to be
        /// activated during wind up.
        /// </summary>
        [Header("References")]
        [SerializeField]
        private GameObject _fx;

        /// <summary>
        /// A transform object indicating where the player will be teleported to when teleporting to this
        /// teleporter.
        /// </summary>
        [SerializeField]
        private Transform _landingPointTransform;

        /// <summary>
        /// The teleporter to which the player must be teleported. This is a one-directional link. To be bidirectional,
        /// the target teleported must target this teleporter as back.
        /// </summary>
        [field: SerializeField]
        public Teleporter TargetTeleporter { get; set; }

        /// <summary>
        /// The absolute position to which the player will be teleported to when teleporting to this teleporter.
        /// </summary>
        public Vector3 LandingPoint => _landingPointTransform.position;

        /// <summary>
        /// A simple reference to the player's transform, for performance reasons.
        /// </summary>
        protected Rigidbody _playerRigidbody;

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
        /// Get global GameObject references once.
        /// </summary>
        private void Start()
        {
            GameObject player = GameObject.FindWithTag("Player");
            if (player == null) throw new UnityException("No player was found.");
            _playerRigidbody = player.GetComponent<Rigidbody>();
        }

        /// <summary>
        /// When the player walks in, the teleporter should start winding up and teleport.
        /// </summary>
        /// <param name="other">The object which should be the player</param>
        private void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag("Player")) return; // Only trigger for player

            if (TargetTeleporter == null) Debug.LogError("No target teleporter set for this teleporter. Won't teleport.");

            // Prevent teleporting to self. Can be used to mark this teleporter as not targeting another one explicitly.
            if (TargetTeleporter == GetComponent<Teleporter>()) return;

            if (_shouldIgnoreNextPlayerEntryTrigger)
            {
                // Do not teleport when the player triggers this because of being teleported to this teleporter
                _shouldIgnoreNextPlayerEntryTrigger = false;
                return;
            }

            _fx.SetActive(true);
            _waitThenTeleportCoroutine = StartCoroutine(WaitThenTeleport()); // Store reference to be able to stop it prematurely
        }

        /// <summary>
        /// Triggered when the player exits prematurely to cancel the teleport, OR when the teleport was successful
        /// (as the player thus also exits it).
        /// </summary>
        /// <param name="other">The object which should be the player</param>
        private void OnTriggerExit(Collider other)
        {
            if (!other.CompareTag("Player")) return; // Only trigger for player

            _fx.SetActive(false);
            if (_waitThenTeleportCoroutine != null) // Can be null when player gets teleported to this teleporter
                StopCoroutine(_waitThenTeleportCoroutine); // Stop coroutine prematurely when player exits during wind up time
        }

        /// <summary>
        /// Waits for <see cref="_teleportWindUpTime"/>, then starts the teleport. This allows the FX to play.
        /// </summary>
        /// <returns>IEnumerator representing coroutine</returns>
        private IEnumerator WaitThenTeleport()
        {
            yield return new WaitForSeconds(_teleportWindUpTime);

            // Teleport the player
            TargetTeleporter.TeleportReceive(_playerRigidbody);
        }

        /// <summary>
        /// Teleport a <see cref="Rigidbody"/> (normally the player) towards this teleporter.
        /// </summary>
        /// <param name="objectRigidbody">The transform to teleport</param>
        public void TeleportReceive(Rigidbody objectRigidbody)
        {
            objectRigidbody.position = LandingPoint;

            // Prevent this action from triggering logic which are intended for when the player walks in.
            _shouldIgnoreNextPlayerEntryTrigger = true;
        }

        /// <summary>
        /// Draw visual aid in the unity editor, when this teleporter is selected.
        /// </summary>
        private void OnDrawGizmosSelected()
        {
            Gizmos.DrawWireSphere(LandingPoint, 0.2f); // Indicate own landingPoint
            if (TargetTeleporter != null)
            {
                Gizmos.DrawSphere(TargetTeleporter.LandingPoint, 0.3f); // Indicate target landingPoint
                Gizmos.DrawLine(LandingPoint, TargetTeleporter.LandingPoint); // Indicate which portals is targeted
            }
        }
    }
}
