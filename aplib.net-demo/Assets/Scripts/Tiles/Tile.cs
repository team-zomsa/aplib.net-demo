using System.Collections.Generic;

namespace Assets.Scripts.Tiles
{
    /// <summary>
    /// Represents a tile.
    /// </summary>
    public abstract class Tile
    {
        /// <summary>
        /// The rotation of the tile. 0 = 0 degrees, 1 = 90 degrees, 2 = 180 degrees, 3 = 270 degrees.
        /// </summary>
        public int rotation = 0;
        
        /// <summary>
        /// The allowed directions for this tile. The index of the list corresponds to the direction. 0 = North, 1 = East, 2 = South, 3 = West.
        /// </summary>
        protected List<bool> AllowedDirections;

        /// <summary>
        /// Checks if the tile can connect in a given direction.
        /// </summary>
        /// <param name="direction">The given direction.</param>
        /// <returns>If the tile can connect to the given direction.</returns>
        public bool CanConnectInDirection(int direction) => AllowedDirections[direction];
    }
}
