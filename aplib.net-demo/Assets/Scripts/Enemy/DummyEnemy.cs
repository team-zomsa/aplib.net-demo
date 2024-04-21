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
    private Bounds _spawnBounds;
    [SerializeField] private bool _respawn;

    protected override void Start()
    {
        _spawnBounds = _spawnArea.Bounds;
    }

    protected override void Die()
    {
        if (_respawn)
            Respawn();
        else
            base.Die();
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
        Health = _maxHealth;
    }
}
