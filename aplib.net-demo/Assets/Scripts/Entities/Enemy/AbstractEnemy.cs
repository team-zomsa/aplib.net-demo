using UnityEngine;

/// <summary>
/// Very basic enemy class.
/// </summary>
[RequireComponent(typeof(PathFind))]
[RequireComponent(typeof(HealthComponent))]
public abstract class AbstractEnemy : MonoBehaviour
{
    [SerializeField] protected int _damagePoints = 25;
    protected HealthComponent _healthComponent;
    private PathFind _pathFind;

    /// <summary>
    /// Initializes the enemy's health to the maximum value.
    /// </summary>
    protected virtual void Awake()
    {
        _pathFind = GetComponent<PathFind>();
        _healthComponent = GetComponent<HealthComponent>();
    }

    /// <summary>
    /// Protected virtual so that it can be overridden by child classes.
    /// </summary>
    protected virtual void Start()
    {
    }

    /// <summary>
    /// Reduces the enemy's health by the specified amount.
    /// If the health reaches zero, the enemy dies.
    /// </summary>
    /// <param name="damage"></param>
    public virtual void TakeDamage(int damage)
    {
        _healthComponent.ReduceHealth(damage);
    }

    /// <summary>
    /// Kills the enemy.
    /// </summary>
    protected virtual void OnDeath()
    {
        Destroy(gameObject);
    }

    /// <summary>
    /// Deals damage to a target.
    /// </summary>
    protected virtual void DealDamage(HealthComponent target)
    {
        target.ReduceHealth(_damagePoints);
    }

    /// <summary>
    /// Update the pathfinding agent.
    /// </summary>
    protected virtual void Update()
    {
        _pathFind.UpdateAgent();
    }
}
