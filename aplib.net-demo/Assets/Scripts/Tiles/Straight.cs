// This program has been developed by students from the bachelor Computer Science at Utrecht
// University within the Software Project course.
// Copyright Utrecht University (Department of Information and Computing Sciences)

using System.Collections.Generic;
using static Assets.Scripts.Tiles.Direction;

namespace Assets.Scripts.Tiles
{
    /// <summary>
    /// Represents a straight tile.
    /// <para/>
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
