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
    protected PathFind _pathFind;

    /// <summary>
    /// Gets the required components and subscribes to the health component's death event.
    /// </summary>
    protected virtual void Awake()
    {
        _pathFind = GetComponent<PathFind>();
        _healthComponent = GetComponent<HealthComponent>();
        _healthComponent.Death += OnDeath;
        _healthComponent.Healed += OnHealed;
        _healthComponent.Hurt += OnHurt;
    }

    /// <summary>
    /// Protected virtual so that it can be overridden by child classes.
    /// </summary>
    protected virtual void Start()
    {
    }

    /// <summary>
    /// Invoked when the health component is hurt.
    /// </summary>
    /// <param name="healthComponent">The health component firing the event.</param>
    /// <param name="amount">The amount of damage taken.</param>
    protected virtual void OnHurt(HealthComponent healthComponent, int amount)
    {
    }

    /// <summary>
    /// Kills the enemy, destroying the GameObject.
    /// Invoked when the health component's death event is fired.
    /// </summary>
    /// <param name="healthComponent">The health component firing the event.</param>
    protected virtual void OnDeath(HealthComponent healthComponent)
    {
        Destroy(gameObject);
    }

    /// <summary>
    /// Invoked when the health component is healed.
    /// </summary>
    /// <param name="healthComponent">The health component firing the event.</param>
    /// <param name="amount">The amount of health restored.</param>
    protected virtual void OnHealed(HealthComponent healthComponent, int amount)
    {
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

    /// <summary>
    /// Unsubscribes from the health component's events.
    /// </summary>
    protected virtual void OnDestroy()
    {
        _healthComponent.Hurt -= OnHurt;
        _healthComponent.Death -= OnDeath;
        _healthComponent.Healed -= OnHealed;
    }
}
