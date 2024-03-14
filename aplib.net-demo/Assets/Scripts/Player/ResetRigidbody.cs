using UnityEngine;

/// <summary>
/// Script to reset the position and velocity of a Rigidbody to its initial state.
/// Requires the object to have a Rigidbody component.
/// </summary>
public class ResetRigidbody : MonoBehaviour
{
    [SerializeField] private Transform _respawnPointTransform;
    private Rigidbody _rigidbodyToRespawn;
    private Vector3 _respawnPoint;

    /// <summary>
    /// Store the initial position.
    /// </summary>
    private void Awake()
    {
        _rigidbodyToRespawn = transform.GetComponent<Rigidbody>();
        _respawnPoint = _respawnPointTransform.position;
    }

    /// <summary>
    /// Reset the position and velocity of the Rigidbody 
    /// </summary>
    public void ResetObject()
    {
        _rigidbodyToRespawn.position = _respawnPoint;
        _rigidbodyToRespawn.velocity = Vector3.zero;
    }
}
