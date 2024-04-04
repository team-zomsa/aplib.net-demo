using UnityEngine;

/// <summary>
/// Dummy basic enemy class to use as target during weapons testing.
/// It respawns after dying.
/// </summary>
public class DummyEnemy : BasicEnemy
{
    /// <summary>
    /// The spawn area for the enemy. 
    /// The size is defined by the local scale of the transform.
    /// </summary>
    [SerializeField] private Transform _spawnArea;
    [SerializeField] private bool _respawn;
    private Vector3 _spawnRange => _spawnArea.localScale / 2.0f;

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
        Vector3 randomRange = new(Random.Range(-_spawnRange.x, _spawnRange.x),
                                    Random.Range(-_spawnRange.y, _spawnRange.y),
                                    Random.Range(-_spawnRange.z, _spawnRange.z));
        transform.position = _spawnArea.position + randomRange;
        Health = _maxHealth;
    }
}
