using System.Collections.Generic;
using static Assets.Scripts.Tiles.Direction;

namespace Assets.Scripts.Tiles
{
    /// <summary>
    /// Represents a straight tile.
    /// <br/><br/>
    /// Default orientation (north):
    /// <code>
    ///
    ///  front
    ///    ↑
    /// ___ ___
    /// | | | |
    /// | | | |
    /// |_| |_|
    /// </code>
    /// </summary>
    public class Straight : Tile
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Straight"/> class.
        /// The default is a vertical tile.
        /// </summary>
        /// <param name="facing">The direction in which the front of the tile should face.</param>
        public Straight(Direction facing = North)
        {
            Facing = facing;

            ConnectingDirections = new List<Direction> { facing, facing.Opposite() };
        }
    }
}
