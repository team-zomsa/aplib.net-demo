// This program has been developed by students from the bachelor Computer Science at Utrecht
// University within the Software Project course.
// Copyright Utrecht University (Department of Information and Computing Sciences)

using System.Collections.Generic;
using static Assets.Scripts.Tiles.Direction;

namespace Assets.Scripts.Tiles
{
    /// <summary>
    /// Represents a crossing tile.
    /// <para/>
    /// Default orientation (north):
    /// <code>
    ///
    ///  front
    ///    ↑
    /// ___ ___
    /// |_| |_|
    /// ___ ___
    /// |_| |_|
    /// </code>
    /// </summary>
    public class Crossing : Tile
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Crossing"/> class.
        /// </summary>
        public Crossing() => ConnectingDirections = new List<Direction> { North, East, South, West };
    }
}
