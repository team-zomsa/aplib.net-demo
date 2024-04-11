using System.Collections.Generic;

namespace Assets.Scripts.Tiles
{
    /// <summary>
    /// Represents a dead end tile.
    /// ___ ___
    /// | | | |
    /// | |_| |
    /// |_____|
    /// </summary>
    public class DeadEnd : Tile
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DeadEnd"/> class.
        /// The default is a north facing tile.
        /// </summary>
        /// <param name="rotate">The amount of times to rotate the tile.</param>
        public DeadEnd(int rotate = 0)
        {
            Rotation = rotate;
            AllowedDirections = new List<bool> { false, false, false, false };

            int index = rotate % 4;

            AllowedDirections[index] = true;
        }
    }
}
