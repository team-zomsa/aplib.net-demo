using System;
using UnityEngine;

/// <summary>
/// Manages the player's points.
/// </summary>
public class PointsManager : MonoBehaviour
{
    /// <summary>
    /// Event that is invoked when points are added.
    /// </summary>
    /// <param name="points">The current amount of points.</param>
    public event Action<int> PointsAdded;

    /// <summary>
    /// The amount of points the player has.
    /// </summary>
    [field: SerializeField]
    public int Points { get; private set; }

    /// <summary>
    /// The singleton instance of this class.
    /// </summary>
    public static PointsManager Instance { get; private set; }

    [SerializeField]
    private PoinstList _pointsList;

    private void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(gameObject);
        else
            Instance = this;
    }

    /// <summary>
    /// Add points to the player's total amount.
    /// </summary>
    /// <param name="points">The amount of points to add.</param>
    public void AddPoints(int points)
    {
        Points += points;
        PointsAdded?.Invoke(Points);
    }
}
