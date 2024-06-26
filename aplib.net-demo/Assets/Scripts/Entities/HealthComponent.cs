// This program has been developed by students from the bachelor Computer Science at Utrecht
// University within the Software Project course.
// Copyright Utrecht University (Department of Information and Computing Sciences)

using System;
using UnityEngine;

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
    public int Health
    {
        get => _health;
        private set => _health = Mathf.Clamp(value, 0, _maxHealth);
    }

    private int _health;

    /// <summary>
    /// The maximum health of the GameObject.
    /// </summary>
    public int MaxHealth { get => _maxHealth; }

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
    /// <param name="amount">The amount of health to be subtracted.</param> 
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
    /// Increase the GameObject's health by the specified amount.
    /// </summary>
    /// <param name="amount">The amount of health to be added.</param>
    public void IncreaseHealth(int amount)
    {
        ReduceHealth(-amount);
    }

    /// <summary>
    /// Resets the GameObject's health to the maximum value.
    /// </summary>
    public void Reset()
        => Health = _maxHealth;

    /// <summary>
    /// Send an event to notify other components that the GameObject has died.
    /// Every other component with an "OnDeath" method will be notified.
    /// </summary>
    private void Die()
        => Death?.Invoke(this);
}
