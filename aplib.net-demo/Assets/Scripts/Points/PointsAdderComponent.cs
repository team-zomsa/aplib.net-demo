using UnityEngine;

public class PointsAdderComponent : MonoBehaviour
{
    [SerializeField]
    protected int _pointAmount = 10;

    /// <summary>
    /// Add points to the player's total amount.
    /// </summary>
    public void SendPoints()
    {
        Debug.Log("Points added: " + _pointAmount);
        PointsManager.Instance.AddPoints(_pointAmount);
    }
}
