using System.Collections.Generic;

namespace Assets.Scripts.Tiles
{
    /// <summary>
    /// Represents a corner tile.
    /// ___ ___
    /// |_| | |
    /// ____| |
    /// |_____|
    /// </summary>
    public class Corner : Tile
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Corner"/> class.
        /// The default is a top-left corner.
        /// </summary>
        /// <param name="rotate">The amount of times to rotate the tile.</param>
        public Corner(int rotate = 0)
        {
            rotate = (rotate + 4) % 4; // Normalise to [0..3]

            Rotation = rotate;
            AllowedDirections = new List<bool> { false, false, false, false };

            int index = (rotate + 3) % 4;
            int nextIndex = (index + 1) % 4;

            AllowedDirections[index] = true;
            AllowedDirections[nextIndex] = true;
        }
    }
}
