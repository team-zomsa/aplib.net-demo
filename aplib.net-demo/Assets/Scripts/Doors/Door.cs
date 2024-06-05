using System;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Assets.Scripts.Doors
{
    /// <summary>
    /// This class handles collisions with the player and makes sure that the parent object (the door) disappears/opens when
    /// the player is in range and the prerequisites are met (the right key).
    /// </summary>
    [RequireComponent(typeof(DoorPointsContributor))]
    public class Door : MonoBehaviour
    {
        public event Action DoorOpened;

        /// <summary>The unique ID of the door, to check whether the player has the right key/ID to open the door.</summary>
        public int DoorId;

        /// <summary>
        /// The color of the door, which is used to determine which key is needed to open it.
        /// </summary>
        public Color Color { get; private set; }

        /// <summary>
        /// The number of doors that have been spawned in the level so far. This is used to give each door a unique ID.
        /// </summary>
        private static int _numberOfDoors;

        private PointsContributorComponent _pointsAdderComponent;

        [SerializeField] private float _minSaturation = 0.5f;

        [SerializeField] private float _maxSaturation = 1f;

        [SerializeField] private float _minValue = 0.4f;

        [SerializeField] private float _maxValue = 1f;

        /// <summary>
        /// Gives the door a unique ID on load.
        /// Randomize the color based on the ID.
        /// Also sets the color of the door.
        /// </summary>
        private void Awake()
        {
            _pointsAdderComponent = GetComponent<PointsContributorComponent>();
            DoorOpened += _pointsAdderComponent.SendPoints;
            DoorId = _numberOfDoors;
            _numberOfDoors++;
            Color = Random.ColorHSV(0f, 1f, _minSaturation, _maxSaturation, _minValue, _maxValue);
            GetComponent<Renderer>().material.color = Color;
            SetDoorText(DoorId.ToString());
        }

        /// <summary>
        /// Checks if the player has the right ID, and destroys the door if true
        /// </summary>
        /// <param name="collidingObject">The object that collides with the collider.</param>
        private void OnTriggerEnter(Collider collidingObject)
        {
            // Delete door if it is triggered by the player and the player has the correct key.
            if (collidingObject.gameObject.CompareTag("Player") && GameObject.Find("KeyRingObject").GetComponent<KeyRing>().KeyQuery(this))
            {
                Open();
                DoorOpened?.Invoke();
            }
        }

        /// <summary>
        /// Matches the given key's ID with the door's ID, returns true if the same.
        /// </summary>
        /// <param name="key">The key that is being checked for the same ID as the door</param>
        /// <returns>True if key id is the same as door id otherwise false.</returns>
        public bool TryOpenDoor(Key key) => key.Id == DoorId;

        /// <summary>
        /// Opens the door by destroying the game object.
        /// </summary>
        private void Open()
        {
            Destroy(gameObject);
        }

        private void SetDoorText(string doorText)
        {
            foreach (TMP_Text textMesh in GetComponentsInChildren<TMP_Text>())
            {
                textMesh.text = doorText;
            }
        }
    }
}
