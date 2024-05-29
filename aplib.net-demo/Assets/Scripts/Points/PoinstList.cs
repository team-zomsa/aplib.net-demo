using UnityEngine;

/// <summary>
/// List of points for PointsManager, used to give a certain point value to certain game objects.
/// </summary>
[CreateAssetMenu(fileName = "PointsList", menuName = "ScriptableObjects/PointsList", order = 1)]
public class PoinstList : ScriptableObject
{
    public readonly int EnemyPoints = 10;

    public readonly int PotionPoints = 5;

    public readonly int EndItemPoints = 25;

    public readonly int KeyPoints = 15;

    public readonly int DoorPoints = 15;

    public readonly int UnusedItemPoints = 10;
}
