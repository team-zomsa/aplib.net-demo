using Assets.Scripts.Items;
using UnityEngine;

/// <summary>
/// This component adds points to the player's score when a key is picked up.
/// </summary>
[RequireComponent(typeof(PickupableKey))]
public class KeyPointsContributor : PointsContributorComponent
{
    private PickupableKey _item;
    private void Awake() => _item = GetComponent<PickupableKey>();

    private void SendPoints(Key item) => PointsManager.Instance.AddPoints(_pointAmount);

    private void OnEnable() => _item.KeyPickedUp += SendPoints;

    private void OnDisable() => _item.KeyPickedUp -= SendPoints;
}
