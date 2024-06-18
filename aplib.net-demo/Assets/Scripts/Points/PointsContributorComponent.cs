using UnityEngine;

/// <summary>
/// This component adds points to the player's total amount.
/// </summary>
public class PointsContributorComponent : MonoBehaviour
{
    [SerializeField]
    protected int _pointAmount = 10;

    /// <summary>
    /// Add points to the player's total amount.
    /// </summary>
    public void SendPoints() => PointsManager.Instance.AddPoints(_pointAmount);

}
