using UnityEngine;

/// <summary>
/// Very basic enemy class.
/// </summary>
public abstract class AbstractEnemy : MonoBehaviour
{
    [SerializeField] protected int _maxHealth = 100;
    public int Health { get; protected set; }

    /// <summary>
    /// Initializes the enemy's health to the maximum value.
    /// </summary>
    protected virtual void Awake()
    {
        Health = _maxHealth;
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
}