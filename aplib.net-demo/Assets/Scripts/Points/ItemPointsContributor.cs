using Assets.Scripts.Items;
using UnityEngine;

[RequireComponent(typeof(PickupableItem))]
public class ItemPointsContributor : PointsContributorComponent
{
    private PickupableItem _item;

    private void Awake() => _item = GetComponent<PickupableItem>();

    private void SendPoints(Item item) => PointsManager.Instance.AddPoints(_pointAmount);

    private void OnEnable() => _item.ItemPickedUp += SendPoints;

    private void OnDisable() => _item.ItemPickedUp -= SendPoints;
}
