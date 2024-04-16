using UnityEngine;

/// <summary>
/// This class handles collisions with the player and makes sure that the parent object (the door) disappears/opens when
/// the players is in range and the prerequisites are met (the right key, or in this case the right player ID).
/// Set the ID in the editor for the collider
/// </summary>
/// <param name="_parent">the door object this script is attached to</param>
/// <param name="doorId">the unique ID of the door, this is used to check whether the player has the right key/ID to open the door</param>

public class DoorCollision : MonoBehaviour
{
    public int doorId;
    [SerializeField] private GameObject _parent;

    /// <summary>
    /// Checks if the player has the right ID, and destroys the door if true
    /// </summary>
    /// <param name="collidingObject">the object that collides with the collider</param>
    private void OnTriggerEnter(Collider collidingObject)
    {
        //Delete door
        if (collidingObject.gameObject.CompareTag("Player") && collidingObject.gameObject.GetComponent<TempItemId>().ItemId == doorId)
        {
            Destroy(_parent);
        }
    }
}
