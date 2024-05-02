using UnityEngine;

/// <summary>
/// Script for player logic.
/// </summary>
[RequireComponent(typeof(HealthComponent))]
[RequireComponent(typeof(Movement))]
[RequireComponent(typeof(ResetRigidbody))]
public class Player : MonoBehaviour
{
    private HealthComponent _healthComponent;
    private ResetRigidbody _resetRigidbody;
    
    /// <summary>
    /// Get the health and resetRb component and subscribe to events.
    /// </summary>
    private void Awake()
    {
        _healthComponent = GetComponent<HealthComponent>();
        _resetRigidbody = GetComponent<ResetRigidbody>();
        _healthComponent.Death += OnDeath;
        _healthComponent.Hurt += OnHurt;
    }

    private void OnHurt(HealthComponent healthComponent, int amount)
    {
        Debug.Log("Player took damage: " + amount);
    }

    /// <summary>
    /// On death, reset the rigidbody and health (respawn).
    /// </summary>
    /// <param name="healthComponent">The health component of the player.</param>.
    private void OnDeath(HealthComponent healthComponent)
    {
        Debug.Log("Player died!");
        _resetRigidbody.ResetObject();
        _healthComponent.Reset();
    }

    private void OnDestroy()
    {
        _healthComponent.Death -= OnDeath;
        _healthComponent.Hurt -= OnHurt;
    }
}
