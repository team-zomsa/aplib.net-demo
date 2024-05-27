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
    private OffMeshLink _offMeshLink;

    public void Awake()
    {
        Key key = gameObject.GetComponent<Key>();

        if (!key) throw new UnityException("Key not found!");

        GameObject[] gameObjects = GameObject.FindGameObjectsWithTag("Door");
        GameObject door = gameObjects.FirstOrDefault(go => go.GetComponent<Door>().TryOpenDoor(key));

        if (!door) throw new UnityException("Door not found!");

        _offMeshLink = door.GetComponent<OffMeshLink>();

        if (!_offMeshLink) throw new UnityException("OffMeshLink not found!");
    }

    private void OnDestroy() => _offMeshLink.activated = true;
}
