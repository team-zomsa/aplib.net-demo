// This program has been developed by students from the bachelor Computer Science at Utrecht
// University within the Software Project course.
// Copyright Utrecht University (Department of Information and Computing Sciences)

using Assets.Scripts.Doors;
using UnityEngine;

/// <summary>
/// This component contributes points when the door is opened.
/// </summary>
[RequireComponent(typeof(Door))]
public class DoorPointsContributor : PointsContributorComponent
{
    private Door _door;
    private void Awake() => _door = GetComponent<Door>();

    private void OnEnable() => _door.DoorOpened += SendPoints;

    private void OnDisable() => _door.DoorOpened -= SendPoints;
}
