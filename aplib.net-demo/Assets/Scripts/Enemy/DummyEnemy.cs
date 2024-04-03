using UnityEngine;

/// <summary>
/// Basic enemy class to use as dummy target during weapons testing.
/// </summary>
public class DummyEnemy : MonoBehaviour
{
    [SerializeField] private float _maxHealth = 100;
    public float Health { get; private set; }
    private Vector3 _spawnPoint;

    private void Awake()
    {
        Health = 100;
        _spawnPoint = transform.position;

    }

    public void TakeDamage(float damage)
    {
        Health -= damage;
        if (Health <= 0)
        {
            ResetAtRandomLocation();
        }
    }

    private void ResetAtRandomLocation()
    {
        transform.position = _spawnPoint + new Vector3(Random.Range(-5, 5), 0, Random.Range(-5, 5));
        Health = _maxHealth;
    }
}
