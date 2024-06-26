// This program has been developed by students from the bachelor Computer Science at Utrecht
// University within the Software Project course.
// Copyright Utrecht University (Department of Information and Computing Sciences)

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
        /// Flags the starting tile.
        /// </summary>
        public bool IsStart { get; set; }

        /// <summary>
        /// The direction in which the front of the tile should face.
        /// </summary>
        public Direction Facing = Direction.North;

        /// <summary>
        /// The directions in which this tile can connect to other tiles.
        /// </summary>
        public List<Direction> ConnectingDirections { get; protected set; }

        /// <summary>
        /// Checks if the tile can connect in a given direction.
        /// </summary>
        /// <param name="direction">The given direction.</param>
        /// <returns>If the tile can connect to the given direction.</returns>
        public bool CanConnectInDirection(Direction direction) => ConnectingDirections.Contains(direction);
    }
}
