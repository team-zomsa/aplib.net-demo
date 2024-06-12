using UnityEngine;

namespace LevelGeneration
{
    /// <summary>
    /// Represents the grid config
    /// </summary>
    [CreateAssetMenu(fileName = "GridConfig", menuName = "ScriptableObjects/GridConfig", order = 1)]
    public class GridConfig : ScriptableObject
    {
        /// <summary>
        /// Represents the use of a seed.
        /// </summary>
        public bool UseSeed;

        /// <summary>
        /// Represents the seed.
        /// </summary>
        public int Seed;

        /// <summary>
        /// Represents the width of the grid in the x-direction.
        /// </summary>
        public int GridWidthX = 10;

        /// <summary>
        /// Represents the height of the grid in the z-direction.
        /// </summary>
        public int GridWidthZ = 10;

        /// <summary>
        /// Represents the amount of rooms.
        /// </summary>
        public int AmountOfRooms = 5;
    }
}
