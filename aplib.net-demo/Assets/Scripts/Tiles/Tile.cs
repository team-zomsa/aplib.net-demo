using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Tiles
{
    /// <summary>
    /// Represents a tile.
    /// </summary>
    public abstract class Tile
    {
        /// <summary>
        /// The game object in the scene, representing this tile.
        /// </summary>
        public GameObject GameObject { get; set; }

        /// <summary>
        /// The direction in which the front of the tile should face.
        /// </summary>
        public Direction Facing = Direction.North;

        /// <summary>
        /// The directions in which this tile can connect to other tiles.
        /// </summary>
        protected List<Direction> ConnectingDirections;

        /// <summary>
        /// Checks if the tile can connect in a given direction.
        /// </summary>
        /// <param name="direction">The given direction.</param>
        /// <returns>If the tile can connect to the given direction.</returns>
        public bool CanConnectInDirection(Direction direction) => ConnectingDirections.Contains(direction);
    }
}
