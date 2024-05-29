using Assets.Scripts.Doors;
using Assets.Scripts.Items;
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
    /// Adds points to the player's total amount based on the class of the parameter.
    /// </summary>
    /// <param name="obj">The object that was picked up.</param>
    // This is probably not a super nice way to do this, but it will work for now.
    public void AddPoints(MonoBehaviour obj)
    {
        switch (obj)
        {
            case AbstractEnemy:
                AddPoints(_pointsList.EnemyPoints);
                Debug.Log("Enemy points added");
                break;

            case RagePotion:
            case HealthPotion:
                AddPoints(_pointsList.PotionPoints);
                Debug.Log("Potion points added");
                break;

            case EndItem:
                AddPoints(_pointsList.EndItemPoints);
                Debug.Log("End item points added");
                break;

            case Key:
                AddPoints(_pointsList.KeyPoints);
                Debug.Log("Key points added");
                break;

            case Door:
                AddPoints(_pointsList.DoorPoints);
                Debug.Log("Door points added");
                break;
        }
    }

    /// <summary>
    /// Grab the health from the player and add points based on the health.
    /// Also adds points based on the amount of unused items left.
    /// </summary>
    public void AddPointsOnGameEnd()
    {
        HealthComponent playerHealth = GameObject.Find("Player").GetComponent<HealthComponent>();
        AddPoints(playerHealth.Health);

        // Not possible unless inventory is changed
        // Inventory inventory = GameObject.Find("InventoryObject").GetComponent<Inventory>();
        // AddPoints(inventory.UnusedItems.Count * _pointsList.UnusedItemPoints);
    }


    /// <summary>
    /// Add points to the player's total amount.
    /// </summary>
    /// <param name="points">The amount of points to add.</param>
    private void AddPoints(int points)
    {
        Points += points;
        PointsAdded?.Invoke(Points);
    }
}
