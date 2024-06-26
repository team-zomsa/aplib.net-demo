// This program has been developed by students from the bachelor Computer Science at Utrecht
// University within the Software Project course.
// Copyright Utrecht University (Department of Information and Computing Sciences)

using Assets.Scripts.Items;
using UnityEngine;

/// <summary>
/// This component adds points to the player's score when the item is picked up.
/// </summary>
[RequireComponent(typeof(PickupableItem))]
public class ItemPointsContributor : PointsContributorComponent
{
    private PickupableItem _item;

    private void Awake() => _item = GetComponent<PickupableItem>();

    private void SendPoints(Item item) => PointsManager.Instance.AddPoints(_pointAmount);

    private void OnEnable() => _item.ItemPickedUp += SendPoints;

    private void OnDisable() => _item.ItemPickedUp -= SendPoints;
}
