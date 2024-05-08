using UnityEngine;

/// <summary>
/// Script for player logic.
/// </summary>
[RequireComponent(typeof(HealthComponent))]
[RequireComponent(typeof(Movement))]
[RequireComponent(typeof(ResetRigidbody))]
[RequireComponent(typeof(EntitySound))]
public class PlayerLogic : MonoBehaviour
{
    private HealthComponent _healthComponent;
    private ResetRigidbody _resetRigidbody;
    private EntitySound _entitySound;

    /// <summary>
    /// Get the health and resetRb component and subscribe to events.
    /// </summary>
    private void Awake()
    {
        _healthComponent = GetComponent<HealthComponent>();
        _resetRigidbody = GetComponent<ResetRigidbody>();
        _healthComponent.Death += OnDeath;
        _healthComponent.Hurt += OnHurt;
        _entitySound = GetComponent<EntitySound>();
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
        _entitySound.OnDeath();
    }

    private void OnDestroy()
    {
        _healthComponent.Death -= OnDeath;
        _healthComponent.Hurt -= OnHurt;
    }
}
