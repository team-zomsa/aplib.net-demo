// This program has been developed by students from the bachelor Computer Science at Utrecht
// University within the Software Project course.
// Copyright Utrecht University (Department of Information and Computing Sciences)

using Assets.Scripts.Doors;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

namespace Assets.Scripts.Items
{
    /// <summary>
    /// Performs logic belonging to when this key is picked up, such as communicating to the NavMesh that the door is open.
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
            GameObject door = gameObjects.FirstOrDefault(go => go.GetComponent<Door>().KeyMatchesDoor(key));

            if (!door) throw new UnityException("Door not found!");

            _offMeshLink = door.GetComponent<OffMeshLink>();

            if (!_offMeshLink) throw new UnityException("OffMeshLink not found!");
        }

        private void OnDestroy() => _offMeshLink.activated = true;
    }
}
