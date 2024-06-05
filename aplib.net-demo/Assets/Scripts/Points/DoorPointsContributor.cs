using Assets.Scripts.Doors;
using UnityEngine;

[RequireComponent(typeof(Door))]
public class DoorPointsContributor : PointsContributorComponent
{
    private Door _door;
    private void Awake() => _door = GetComponent<Door>();

    private void OnEnable() => _door.DoorOpened += SendPoints;

    private void OnDisable() => _door.DoorOpened -= SendPoints;
}
