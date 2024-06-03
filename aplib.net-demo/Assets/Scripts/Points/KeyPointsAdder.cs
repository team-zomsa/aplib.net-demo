using Assets.Scripts.Items;
using UnityEngine;

public class KeyPointsAdder : PointsAdderComponent
{
    private void Awake()
    {
        PickupableKey item = GetComponent<PickupableKey>();
        item.KeyPickedUp += SendPoints;
    }

    private void SendPoints(Key item) 
    {
        Debug.Log("Points added: " + _pointAmount);
        PointsManager.Instance.AddPoints(_pointAmount);
    }
}
