using System.Collections.Generic;

namespace Assets.Scripts.Tiles
{
    /// <summary>
    /// Represents a T-section tile.
    /// _______
    /// |_____|
    /// ___ ___
    /// |_| |_|
    /// </summary>
    public class TSection : Tile
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TSection"/> class.
        /// The default is a T-section with the top side closed.
        /// </summary>
        /// <param name="rotate">The amount of times to rotate the tile.</param>
        public TSection(int rotate = 0)
        {
            _allowedDirections = new List<bool> { true, true, true, true };

            int index = rotate % 4;

            _allowedDirections[index] = false;
        }
    }
}
