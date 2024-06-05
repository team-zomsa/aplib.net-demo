using UnityEngine;

[RequireComponent(typeof(HealthComponent))]
public class EnemyPointsAdder : PointsAdderComponent
{
    private HealthComponent _enemyHealth;

    /// <summary>
    /// Grab the enemy health component.
    /// </summary>
    private void Awake()
    {
        _enemyHealth = GetComponent<HealthComponent>();
    }

    private void SendPoints(HealthComponent healthComponent)
    {
        Debug.Log("Points added: " + _pointAmount);
        PointsManager.Instance.AddPoints(_pointAmount);
    }

    private void OnEnable()
    {
        _enemyHealth.Death += SendPoints;
    }

    private void OnDisable()
    {
        _enemyHealth.Death -= SendPoints;
    }
}
