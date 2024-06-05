using Assets.Scripts.Items;
using UnityEngine;

[RequireComponent(typeof(PickupableItem))]
public class ItemPointsAdder : PointsAdderComponent
{
    private PickupableItem _item;

    private void Awake()
    {
        _item = GetComponent<PickupableItem>();
    }

    private void SendPoints(Item item)
    {
        Debug.Log("Points added: " + _pointAmount);
        PointsManager.Instance.AddPoints(_pointAmount);
    }

    private void OnEnable()
    {
        _item.ItemPickedUp += SendPoints;
    }

    private void OnDisable()
    {
        _item.ItemPickedUp -= SendPoints;
    }
}
