// This program has been developed by students from the bachelor Computer Science at Utrecht
// University within the Software Project course.
// Copyright Utrecht University (Department of Information and Computing Sciences)

using UnityEngine;

namespace LevelGeneration
{
    /// <summary>
    /// Represents the room objects.
    /// </summary>
    [CreateAssetMenu(fileName = "RoomObjects", menuName = "ScriptableObjects/RoomObjects", order = 1)]
    public class RoomObjects : ScriptableObject
    {
        /// <summary>
        /// Represents the <see cref="Corner"/> object.
        /// </summary>
        public GameObject Corner;

        /// <summary>
        /// Represents the <see cref="StartCorner"/> object.
        /// </summary>
        public GameObject StartCorner;

        /// <summary>
        /// Represents the <see cref="Crossing"/> object.
        /// </summary>
        public GameObject Crossing;

        /// <summary>
        /// Represents the <see cref="StartCrossing"/> object.
        /// </summary>
        public GameObject StartCrossing;

        /// <summary>
        /// Represents the <see cref="DeadEnd"/> object.
        /// </summary>
        public GameObject DeadEnd;

        /// <summary>
        /// Represents the <see cref="StartDeadEnd"/> object.
        /// </summary>
        public GameObject StartDeadEnd;

        /// <summary>
        /// Represents the <see cref="Empty"/> object.
        /// </summary>
        public GameObject Empty;

        /// <summary>
        /// Represents the <see cref="Room"/> object.
        /// </summary>
        public GameObject Room;

        /// <summary>
        /// Represents the <see cref="StartRoom"/> object.
        /// </summary>
        public GameObject StartRoom;

        /// <summary>
        /// Represents the <see cref="Straight"/> object.
        /// </summary>
        public GameObject Straight;

        /// <summary>
        /// Represents the <see cref="StartStraight"/> object.
        /// </summary>
        public GameObject StartStraight;

        /// <summary>
        /// Represents the <see cref="TSection"/> object.
        /// </summary>
        public GameObject TSection;

        /// <summary>
        /// Represents the <see cref="StartTSection"/> object.
        /// </summary>
        public GameObject StartTSection;
    }
}
