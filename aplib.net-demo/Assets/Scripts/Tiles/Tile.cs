using System.Collections.Generic;

namespace Assets.Scripts.Tiles
{
    /// <summary>
    /// Represents a tile.
    /// </summary>
    public abstract class Tile
    {
        /// <summary>
        /// The allowed directions for this tile. The index of the list corresponds to the direction. 0 = North, 1 = East, 2 = South, 3 = West.
        /// </summary>
        protected List<bool> _allowedDirections;

        /// <summary>
        /// Checks if the tile can connect in a given direction.
        /// </summary>
        /// <param name="direction">The given direction.</param>
        /// <returns>If the tile can connect to the given direction.</returns>
        public bool CanConnectInDirection(int direction) => _allowedDirections[direction];
    }
}
