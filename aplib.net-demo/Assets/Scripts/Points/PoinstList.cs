using UnityEngine;

/// <summary>
/// List of points for PointsManager, used to give a certain point value to certain game objects.
/// </summary>
[CreateAssetMenu(fileName = "PointsList", menuName = "ScriptableObjects/PointsList", order = 1)]
public class PoinstList : ScriptableObject
{
    public int EnemyPointsAmount = 10;

    public int PotionPointsAmount = 5;

    public int ElixirPointsAmount = 25;

    public int KeyPointsAmount = 15;

    public int DoorPointsAmount = 15;
}
