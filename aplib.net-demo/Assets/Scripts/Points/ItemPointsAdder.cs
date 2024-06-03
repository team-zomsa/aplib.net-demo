using Assets.Scripts.Items;
using UnityEngine;

public class ItemPointsAdder : PointsAdderComponent
{
    private void Awake()
    {
        PickupableItem item = GetComponent<PickupableItem>();
        item.ItemPickedUp += SendPoints;
    }

    private void SendPoints(Item item) 
    {
        Debug.Log("Points added: " + _pointAmount);
        PointsManager.Instance.AddPoints(_pointAmount);
    }
}
