using System;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

namespace Entities
{
    /// <summary>
    /// Component that allows an entity to respawn by resetting its position and velocity.
    /// </summary>
    public class RespawnableComponent : MonoBehaviour
    {
        [SerializeField]
        public Area spawnArea;
        private Bounds _spawnBounds;
        private Rigidbody _rigidbodyToRespawn;

        /// <summary>
        /// Event that is triggered when an entity respawn occurs.
        /// </summary>
        /// <param>The RespawnableComponent instance that triggered the respawn.</param>
        public event Action<RespawnableComponent> RespawnEvent;

        /// <summary>
        /// Store the initial position.
        /// </summary>
        private void Awake()
        {
            _rigidbodyToRespawn = transform.GetComponent<Rigidbody>();
            if (spawnArea is null)
            {
                Debug.LogWarning($"No spawn area found for {name}. Defaulting to spawn position.");
                _spawnBounds = new Bounds(transform.position, Vector3.zero);
            }
            else
            {
                _spawnBounds = spawnArea.Bounds;
            }
        }

        /// <summary>
        /// Reset the position and velocity of the Rigidbody
        /// </summary>
        public void Respawn()
        {
            Vector3 randomPoint = new(Random.Range(_spawnBounds.min.x, _spawnBounds.max.x),
                Random.Range(_spawnBounds.min.y, _spawnBounds.max.y),
                Random.Range(_spawnBounds.min.z, _spawnBounds.max.z));

            // If the entity has a Rigidbody, reset its position and velocity.
            if (_rigidbodyToRespawn != null)
            {
                _rigidbodyToRespawn.position = randomPoint;
                _rigidbodyToRespawn.velocity = Vector3.zero;
            }
            else
            {
                transform.position = randomPoint;
            }

            RespawnEvent?.Invoke(this);
        }
    }
}
