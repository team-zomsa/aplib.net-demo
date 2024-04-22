using System.Collections.Generic;
using static Assets.Scripts.Tiles.Direction;

namespace Assets.Scripts.Tiles
{
    /// <summary>
    /// Represents a dead end tile.
    /// <para/>
    /// Default orientation (north):
    /// <code>
    ///
    ///  front
    ///    ↑
    /// ___ ___
    /// | | | |
    /// | |_| |
    /// |_____|
    /// </code>
    /// </summary>
    public class DeadEnd : Tile
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DeadEnd"/> class.
        /// The default is a north facing tile.
        /// </summary>
        /// <param name="facing">The direction in which the front of the tile should face.</param>
        public DeadEnd(Direction facing = North)
        {
            Facing = facing;
            ConnectingDirections = new List<Direction> { facing };
        }
    }
}
