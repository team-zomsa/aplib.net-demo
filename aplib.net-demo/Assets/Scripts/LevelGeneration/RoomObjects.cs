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
        /// Represents the <see cref="Crossing"/> object.
        /// </summary>
        public GameObject Crossing;

        /// <summary>
        /// Represents the <see cref="DeadEnd"/> object.
        /// </summary>
        public GameObject DeadEnd;

        /// <summary>
        /// Represents the <see cref="Empty"/> object.
        /// </summary>
        public GameObject Empty;

        /// <summary>
        /// Represents the <see cref="Room"/> object.
        /// </summary>
        public GameObject Room;

        /// <summary>
        /// Represents the <see cref="Straight"/> object.
        /// </summary>
        public GameObject Straight;

        /// <summary>
        /// Represents the <see cref="TSection"/> object.
        /// </summary>
        public GameObject TSection;
    }
}
