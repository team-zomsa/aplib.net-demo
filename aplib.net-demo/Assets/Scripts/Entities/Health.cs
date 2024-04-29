using UnityEngine;

/// <summary>
/// Health component that can be added to any GameObject.
/// Keeps track of the GameObject's health and whether it is dead.
/// When it is dead, it should fire an event to notify other components.
/// </summary>
public class HealthComponent : MonoBehaviour
{
    [SerializeField] private int _maxHealth = 100;
    public int Health { get; private set; }
    public bool IsDead => Health <= 0;

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
        if (Health <= 0)
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
        SendMessage("OnDeath", SendMessageOptions.DontRequireReceiver);
    }
}
