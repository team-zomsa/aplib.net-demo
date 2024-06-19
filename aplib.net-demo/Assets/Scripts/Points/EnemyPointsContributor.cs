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
