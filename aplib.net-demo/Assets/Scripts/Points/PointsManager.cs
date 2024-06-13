using System;
using UnityEngine;

/// <summary>
/// Manages the player's points.
/// </summary>
public class PointsManager
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
    public static PointsManager Instance
    {
        get
        {
            _instance ??= new PointsManager();
            return _instance;
        }
    }

    private static PointsManager _instance;

    [SerializeField]
    private readonly int _unusedItemPoints = 5;

    /// <summary>
    /// Private constructor to prevent instantiation by other classes.
    /// </summary>
    private PointsManager() { }

    /// <summary>
    /// Grab the health from the player and add points based on the health.
    /// Also adds points based on the amount of unused items left.
    /// </summary>
    public void AddPointsOnGameEnd()
    {
        HealthComponent playerHealth = GameObject.Find("Player").GetComponent<HealthComponent>();
        AddPoints(playerHealth.Health);

        // Add points for each unused item in the inventory
        Inventory inventory = GameObject.Find("InventoryObject").GetComponent<Inventory>();
        AddPoints(inventory.UnusedItems() * _unusedItemPoints);

        CanvasManager.Instance.SetPointsText(Points);
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
