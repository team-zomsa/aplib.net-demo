using System.Collections.Generic;
using static Assets.Scripts.Tiles.Direction;

namespace Assets.Scripts.Tiles
{
    /// <summary>
    /// Represents a corner tile.
    /// <para/>
    /// Default orientation (north):
    /// <code>
    ///
    ///  front
    ///    ↑
    /// ___ ___
    /// |_| | |
    /// ____| |
    /// |_____|
    /// </code>
    /// </summary>
    public class Corner : Tile
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Corner"/> class.
        /// The default is a top-left corner.
        /// </summary>
        /// <param name="facing">The direction in which the front of the tile should face.</param>
        public Corner(Direction facing = North)
        {
            Facing = facing;
            ConnectingDirections = new List<Direction> { facing, facing.RotateLeft() };
        }
    }
}
