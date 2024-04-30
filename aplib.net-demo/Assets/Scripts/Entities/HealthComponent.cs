using UnityEngine;
using System;

/// <summary>
/// Health component that can be added to any GameObject.
/// Keeps track of the GameObject's health and whether it is dead.
/// When it is dead, it should fire an event to notify other components.
/// </summary>
public class HealthComponent : MonoBehaviour
{
    /// <summary>
    /// Event that is fired when the GameObject dies.
    /// </summary>
    public event Action<HealthComponent> Death;

    /// <summary>
    /// Event that is fired when the GameObject is hurt, with the amount of damage taken.
    /// </summary>
    public event Action<HealthComponent, int> Hurt;

    /// <summary>
    /// Event that is fired when the GameObject is healed, with the amount of health restored.
    /// </summary>
    public event Action<HealthComponent, int> Healed;

    /// <summary>
    /// Whether the GameObject is dead.
    /// </summary>
    public bool IsDead => Health <= 0;

    /// <summary>
    /// The current health of the GameObject.
    /// </summary>
    public int Health { get; private set; }
    
    [SerializeField]
    private int _maxHealth = 100;

    private void Awake()
    {
        Health = _maxHealth;
    }
    
    /// <summary>
    /// Reduces the GameObject's health by the specified amount.
    /// If the health reaches zero, the GameObject dies.
    /// Healing can be done by passing a negative value.
    /// </summary>
    /// <param name="amount">The amount </param>
    public void ReduceHealth(int amount)
    {
        Health -= amount;
        if (amount >= 0)
            Hurt?.Invoke(this, amount);
        else
            Healed?.Invoke(this, -amount);
        
        if (IsDead)
            Die();
    }

    
    /// <summary>
    /// Resets the GameObject's health to the maximum value.
    /// </summary>
    public void Reset()
    {
        Health = _maxHealth;
    }

    /// <summary>
    /// Send an event to notify other components that the GameObject has died.
    /// Every other component with an "OnDeath" method will be notified.
    /// </summary>
    private void Die()
    {
        Death?.Invoke(this);
    }
}
