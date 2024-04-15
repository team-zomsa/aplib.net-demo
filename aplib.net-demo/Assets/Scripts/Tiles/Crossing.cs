using System.Collections.Generic;
using static Assets.Scripts.Tiles.Direction;

namespace Assets.Scripts.Tiles
{
    /// <summary>
    /// Represents a crossing tile.
    /// <br/><br/>
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
