using System.Collections.Generic;

namespace Assets.Scripts.Tiles
{
    /// <summary>
    /// Represents a room tile.
    /// </summary>
    public class Room : Tile
    {
        public List<Direction> Doors;

        /// <summary>
        /// Initializes a new instance of the <see cref="Room"/> class.
        /// </summary>
        /// <param name="connectingDirections">The allowed directions of the tile.</param>
        /// <param name="doors">The doors of the room.</param>
        public Room(List<Direction> connectingDirections, List<Direction> doors)
        {
            ConnectingDirections = connectingDirections;
            Doors = doors;
        }
    }
}
