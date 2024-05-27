using Assets.Scripts.Doors;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// Attach this class to make object pickupable.
/// </summary>
[RequireComponent(typeof(Key))]
public class BeforeDestroyKey : MonoBehaviour
{
    private GameObject _door;

    private Key _key;

    public void Awake()
    {
        _key = gameObject.GetComponent<Key>();

        if (!_key) throw new UnityException("Key not found!");

        GameObject[] gameObjects = GameObject.FindGameObjectsWithTag("Door");
        _door = gameObjects.FirstOrDefault(go => go.GetComponent<Door>().TryOpenDoor(_key));

        if (!_door) throw new UnityException("Door not found!");
    }

    private void OnDestroy()
    {
        OffMeshLink offMeshLink = _door.GetComponent<OffMeshLink>();
        offMeshLink.activated = true;
        Debug.Log("Door activated");
    }
}
