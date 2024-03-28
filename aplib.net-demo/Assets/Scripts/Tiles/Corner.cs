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
        /// The default is a bottom-right corner.
        /// </summary>
        /// <param name="rotate">The amount of times to rotate the tile.</param>
        public Corner(int rotate = 0)
        {
            if (rotate == 0)
            {
                _allowedDirections = new List<bool> { false, true, true, false };
            }
            else if (rotate == 1)
            {
                _allowedDirections = new List<bool> { false, false, true, true };
            }
            else if (rotate == 2)
            {
                _allowedDirections = new List<bool> { true, false, false, true };
            }
            else
            {
                _allowedDirections = new List<bool> { true, true, false, false };
            }
        }
    }
}
