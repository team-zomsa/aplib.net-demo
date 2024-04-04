using UnityEngine;

/// <summary>
/// Dummy basic enemy class to use as target during weapons testing.
/// It respawns after dying.
/// </summary>
public class DummyEnemy : BasicEnemy
{
    private Vector3 _spawnPoint;

    /// <summary>
    /// Initializes the enemy's health to the maximum value.
    /// Also stores the initial position as the spawn point.
    /// </summary>
    protected override void Awake()
    {
        base.Awake();
        _spawnPoint = transform.position;
    }

    protected override void Die() 
    {
        Respawn();
    }

    /// <summary>
    /// Respawns the enemy at a random position and resets its health.
    /// </summary>
    private void Respawn()
    {
        transform.position = _spawnPoint + new Vector3(Random.Range(-5, 5), 0, Random.Range(-5, 5));
        Health = _maxHealth;
    }
}
