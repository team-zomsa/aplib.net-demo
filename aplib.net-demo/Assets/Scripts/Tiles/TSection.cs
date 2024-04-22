using System.Collections.Generic;
using static Assets.Scripts.Tiles.Direction;

namespace Assets.Scripts.Tiles
{
    /// <summary>
    /// Represents a T-section tile.
    /// <para/>
    /// Default orientation (north):
    /// <code>
    ///
    ///  front
    ///    ↑
    /// ___ ___
    /// |_| |_|
    /// _______
    /// |_____|
    /// </code>
    /// </summary>
    public class TSection : Tile
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TSection"/> class.
        /// The default is a T-section with the bottom side closed.
        /// </summary>
        /// <param name="facing">The direction in which the front of the tile should face.</param>
        public TSection(Direction facing = North)
        {
            Facing = facing;
            ConnectingDirections = new List<Direction> { facing, facing.RotateLeft(), facing.RotateRight() };
        }
    }
}
