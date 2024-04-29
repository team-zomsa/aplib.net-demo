using UnityEngine;

/// <summary>
/// Very basic enemy class.
/// </summary>
[RequireComponent(typeof(PathFind))]
public abstract class AbstractEnemy : MonoBehaviour
{
    [SerializeField] protected int _maxHealth = 100;
    [SerializeField] protected int _damagePoints = 25;
    public int Health { get; protected set; }
    private PathFind _pathFind;

    /// <summary>
    /// Initializes the enemy's health to the maximum value.
    /// </summary>
    protected virtual void Awake()
    {
        Health = _maxHealth;
        _pathFind = GetComponent<PathFind>();
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
        Health -= damage;
        if (Health <= 0)
            Die();
    }

    /// <summary>
    /// Kills the enemy.
    /// </summary>
    protected virtual void Die()
    {
        Destroy(gameObject);
    }

    /// <summary>
    /// Deals damage to a target.
    /// (Player does not have health yet!)
    /// </summary>
    protected virtual void DealDamage(Object target)
    {
        // TODO:: implement when Health component is added
        // target.TakeDamage(_damagePoints);
    }

    /// <summary>
    /// Update the pathfinding agent.
    /// </summary>
    protected virtual void Update()
    {
        _pathFind.UpdateAgent();
    }
}
