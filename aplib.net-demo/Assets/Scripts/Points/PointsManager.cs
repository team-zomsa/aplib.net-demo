using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

/// <summary>
/// Manages the player's points.
/// </summary>
public class PointsManager : Singleton<PointsManager>
{
    /// <summary>
    /// Event that is invoked when points are added.
    /// </summary>
    /// <param name="points">The current amount of points.</param>
    public event Action<int> PointsAdded;

    /// <summary>
    /// The amount of points the player has.
    /// </summary>
    public int Points { get; private set; } = 0;

    private readonly int _unusedItemPoints = 5;

    private void Start() => GameManager.Instance.GameWon += AddPointsOnGameEnd;

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

        DisplayPoints();
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

    private void DisplayPoints()
    {
        List<TextMeshProUGUI> textMeshes = GameCanvas.Instance.WinScreenCanvas.GetComponentsInChildren<TextMeshProUGUI>().ToList();
        TextMeshProUGUI pointsText = textMeshes.Find(textMesh => textMesh.name == "Points");
        pointsText.text = "Points: " + Points;
    }
}
