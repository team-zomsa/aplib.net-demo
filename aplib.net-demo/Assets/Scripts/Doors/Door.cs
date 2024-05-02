using UnityEngine;

/// <summary>
/// This class handles collisions with the player and makes sure that the parent object (the door) disappears/opens when
/// the player is in range and the prerequisites are met (the right key).
/// </summary>
public class Door : MonoBehaviour
{
    /// <summary>
    /// The number of doors that have been spawned in the level so far. This is used to give each door a unique ID.
    /// </summary>
    private static int _numberOfDoors;

    /// <summary>The unique ID of the door, to check whether the player has the right key/ID to open the door.</summary>
    private int _doorId;

    /// <summary>Gives the door a unique ID on load.</summary>
    private void Awake()
    {
        _doorId = _numberOfDoors;
        _numberOfDoors++;
    }

    /// <summary>
    /// Checks if the player has the right ID, and destroys the door if true
    /// </summary>
    /// <param name="collidingObject">The object that collides with the collider.</param>
    private void OnTriggerEnter(Collider collidingObject)
    {
        // Delete door if it is triggered by the player.
        if (collidingObject.gameObject.CompareTag("Player"))
            Destroy(transform.parent.gameObject);

        //Once the keys and doors are given matching id's, we can use this
        /*
          if (collidingObject.gameObject.CompareTag("Player") && collidingObject.GetComponent<KeyRing>().KeyQuery(this))
            {
              Destroy(transform.parent.gameObject);
            }
        */
    }
    
    /// <summary>
    /// Matches the given key's ID with the door's ID, returns true if the same
    /// </summary>
    /// <param name="k">The key that is being checked for the same ID as the door</param>
    /// <returns></returns>
    public bool TryOpenDoor(Key k) => k.Id == _doorId;
}
