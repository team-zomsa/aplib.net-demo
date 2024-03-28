using System.Collections.Generic;

namespace Assets.Scripts.Tiles
{
    /// <summary>
    /// Represents a corner tile.
    /// </summary>
    public class Corner : Tile
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Corner"/> class.
        /// The default is a top-right corner.
        /// </summary>
        /// <param name="rotate">The amount of times to rotate the tile.</param>
        public Corner(int rotate = 0)
        {
            _allowedDirections = new List<bool> { false, false, false, false };

            int index = rotate % 4;
            int nextIndex = (index + 1) % 4;

            _allowedDirections[index] = true;
            _allowedDirections[nextIndex] = true;
        }
    }
}
