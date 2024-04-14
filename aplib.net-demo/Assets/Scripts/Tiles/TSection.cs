using System.Collections.Generic;

namespace Assets.Scripts.Tiles
{
    /// <summary>
    /// Represents a T-section tile.
    /// ___ ___
    /// |_| |_|
    /// _______
    /// |_____|
    /// </summary>
    public class TSection : Tile
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TSection"/> class.
        /// The default is a T-section with the top side opened.
        /// </summary>
        /// <param name="rotate">The amount of times to rotate the tile.</param>
        public TSection(int rotate = 0)
        {
            rotate = (rotate + 4) % 4; // Normalise to [0..3]
            Rotation = rotate;
            AllowedDirections = new List<bool> { true, true, true, true };

            int index = (rotate + 2) % 4;

            AllowedDirections[index] = false;
        }
    }
}
