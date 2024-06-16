using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(HealthComponent))]
public class GameOverOnDeath : MonoBehaviour
{
    private HealthComponent _healthComponent;

    private void Awake() => _healthComponent = GetComponent<HealthComponent>();

    private void OnEnable() => _healthComponent.Death += OnDeath;

    private void OnDisable() => _healthComponent.Death -= OnDeath;

    private void OnDeath(HealthComponent healthComponent) => GameManager.Instance.TriggerGameOver();
}
