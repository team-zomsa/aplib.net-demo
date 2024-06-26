// This program has been developed by students from the bachelor Computer Science at Utrecht
// University within the Software Project course.
// Copyright Utrecht University (Department of Information and Computing Sciences)

using UnityEngine;

/// <summary>
/// This component triggers the game over event when the entity dies.
/// </summary>
[RequireComponent(typeof(HealthComponent))]
public class GameOverOnDeath : MonoBehaviour
{
    private HealthComponent _healthComponent;

    private void Awake() => _healthComponent = GetComponent<HealthComponent>();

    private void OnEnable() => _healthComponent.Death += OnDeath;

    private void OnDisable() => _healthComponent.Death -= OnDeath;

    private void OnDeath(HealthComponent healthComponent) => GameManager.Instance.TriggerGameOver();
}
