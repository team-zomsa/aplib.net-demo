using UnityEngine;

/// <summary>
/// Dummy basic enemy class to use as target during weapons testing.
/// It respawns after dying.
/// </summary>
public class DummyEnemy : AbstractEnemy
{
    /// <summary>
    /// The spawn area for the enemy. 
    /// </summary>
    [SerializeField] private Area _spawnArea;
    [SerializeField] private bool _respawn = true;
    private Bounds _spawnBounds;

    /// <summary>
    /// Set the spawn bounds.
    /// This needs to happen in start, because the spawn area is not initialized yet in Awake.
    /// </summary>
    protected override void Start()
    {
        _spawnArea ??= transform.parent.GetComponentInChildren<Area>();
        if (_spawnArea is null)
            Debug.LogError("No spawn area found for enemy " + name);
        else
            _spawnBounds = _spawnArea.Bounds;
    }

    protected override void OnDeath(HealthComponent healthComponent)
    {
        if (_respawn)
            Respawn();
        else
            base.OnDeath(healthComponent);
    }

    /// <summary>
    /// Respawns the enemy at a random position and resets its health.
    /// </summary>
    private void Respawn()
    {
        Vector3 randomPoint = new Vector3(Random.Range(_spawnBounds.min.x, _spawnBounds.max.x),
                                          Random.Range(_spawnBounds.min.y, _spawnBounds.max.y),
                                          Random.Range(_spawnBounds.min.z, _spawnBounds.max.z));
        transform.position = randomPoint;
        _healthComponent.Reset();
    }
}
