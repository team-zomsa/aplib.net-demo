using Entities;
using UnityEngine;

/// <summary>
/// Script for player logic.
/// </summary>
[RequireComponent(typeof(HealthComponent))]
[RequireComponent(typeof(Movement))]
[RequireComponent(typeof(RespawnableComponent))]
[RequireComponent(typeof(Animator))]
public class PlayerLogic : MonoBehaviour
{
    [SerializeField]
    private bool _respawnOnDeath = true;
    private HealthComponent _healthComponent;
    private RespawnableComponent _respawnableComponent;
    public Vector3 EyesPosition => transform.position + new Vector3(0, 1, 0);


    /// <summary>
    /// Get the health and resetRb component and subscribe to events.
    /// </summary>
    private void Awake()
    {
        _healthComponent = GetComponent<HealthComponent>();
        _respawnableComponent = GetComponent<RespawnableComponent>();
        _healthComponent.Death += OnDeath;
        _respawnableComponent.RespawnEvent += OnSelfRespawn;
        _healthComponent.Hurt += OnHurt;
    }

    private void OnHurt(HealthComponent healthComponent, int amount)
    {
        Debug.Log("Player took damage: " + amount);
    }

    private void OnSelfRespawn(RespawnableComponent respawnableComponent)
    {
        _healthComponent.Reset();
    }

    /// <summary>
    /// On death, reset the rigidbody and health (respawn).
    /// </summary>
    /// <param name="healthComponent">The health component of the player.</param>.
    private void OnDeath(HealthComponent healthComponent)
    {
        if (_respawnOnDeath) _respawnableComponent.Respawn();
    }

    private void OnDestroy()
    {
        _healthComponent.Death -= OnDeath;
        _healthComponent.Hurt -= OnHurt;
    }
}
