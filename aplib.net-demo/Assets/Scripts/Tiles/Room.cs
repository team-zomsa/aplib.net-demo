using System.Collections.Generic;

namespace Assets.Scripts.Tiles
{
    /// <summary>
    /// Represents a room tile.
    /// </summary>
    public class Room : Tile
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Room"/> class.
        /// </summary>
        /// <param name="allowedDirections">The allowed directions of the tile.</param>
        public Room(List<bool> allowedDirections) => AllowedDirections = allowedDirections;
    }
}
