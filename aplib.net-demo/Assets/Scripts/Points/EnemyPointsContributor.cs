// This program has been developed by students from the bachelor Computer Science at Utrecht
// University within the Software Project course.
// Copyright Utrecht University (Department of Information and Computing Sciences)

using UnityEngine;

/// <summary>
/// This component adds points to the player when the enemy dies.
/// </summary>
[RequireComponent(typeof(HealthComponent))]
public class EnemyPointsContributor : PointsContributorComponent
{
    private HealthComponent _enemyHealth;

    /// <summary>
    /// Grab the enemy health component.
    /// </summary>
    private void Awake() => _enemyHealth = GetComponent<HealthComponent>();

    private void SendPoints(HealthComponent healthComponent) => PointsManager.Instance.AddPoints(_pointAmount);

    private void OnEnable() => _enemyHealth.Death += SendPoints;

    private void OnDisable() => _enemyHealth.Death -= SendPoints;
}
