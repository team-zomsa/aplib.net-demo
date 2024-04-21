using UnityEngine;

/// <summary>
/// This class handles collisions with the player and makes sure that the parent object (the door) disappears/opens when
/// the player is in range and the prerequisites are met (the right key, or in this case the right player ID).
/// Set the ID in the editor for the collider
/// </summary>
public class DoorCollision : MonoBehaviour
{
    /// <summary>The unique ID of the door, to check whether the player has the right key/ID to open the door</summary>
    public int doorId;
    /// <summary>the door object this script is attached to</summary>
    [SerializeField] private GameObject parent;

    /// <summary>
    /// Checks if the player has the right ID, and destroys the door if true
    /// </summary>
    /// <param name="collidingObject">the object that collides with the collider</param>
    private void OnTriggerEnter(Collider collidingObject)
    {
        //Delete door
        if (collidingObject.gameObject.CompareTag("Player") && collidingObject.gameObject.GetComponent<TempItemId>().itemId == doorId)
        {
            Destroy(parent);
        }
    }
}