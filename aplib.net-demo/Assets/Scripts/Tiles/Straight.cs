using System.Collections.Generic;

namespace Assets.Scripts.Tiles
{
    /// <summary>
    /// Represents a straight tile.
    /// ___ ___
    /// | | | |
    /// | | | |
    /// |_| |_|
    /// </summary>
    public class Straight : Tile
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Straight"/> class.
        /// The default is a vertical tile.
        /// </summary>
        /// <param name="rotate">The amount of times to rotate the tile.</param>
        public Straight(int rotate = 0)
        {
            rotation = rotate;
            bool isVertical = rotate % 2 == 0;

            AllowedDirections = new List<bool> { isVertical, !isVertical, isVertical, !isVertical };
        }
    }
}
