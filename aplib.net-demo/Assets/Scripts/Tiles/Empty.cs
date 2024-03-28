using System.Collections.Generic;

namespace Assets.Scripts.Tiles
{
    /// <summary>
    /// Represents an empty tile.
    /// </summary>
    public class Empty : Tile
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Empty"/> class.
        /// </summary>
        public Empty() => _allowedDirections = new List<bool> { false, false, false, false };
    }
}
