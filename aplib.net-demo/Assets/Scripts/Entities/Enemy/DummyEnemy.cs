using Entities;
using UnityEngine;

/// <summary>
/// Dummy basic enemy class to use as target during weapons testing.
/// It respawns after dying.
/// </summary>
[RequireComponent(typeof(RespawnableComponent))]
public class DummyEnemy : AbstractEnemy
{
    /// <summary>
    /// The spawn area for the enemy.
    /// </summary>
    [SerializeField] private bool _respawn = true;
    private RespawnableComponent _respawnableComponent;

    /// <summary>
    /// Set the spawn bounds.
    /// This needs to happen in start, because the spawn area is not initialized yet in Awake.
    /// </summary>
    protected override void Start()
    {
        _respawnableComponent = GetComponent<RespawnableComponent>();
        _respawnableComponent.RespawnEvent += OnRespawn;
    }

    protected override void OnDeath(HealthComponent healthComponent)
    {
        if (_respawn)
            _respawnableComponent.Respawn();
        else
            base.OnDeath(healthComponent);
    }

    /// <summary>
    /// Resets its health.
    /// </summary>
    private void OnRespawn(RespawnableComponent _)
    {
        _healthComponent.Reset();
    }
}
