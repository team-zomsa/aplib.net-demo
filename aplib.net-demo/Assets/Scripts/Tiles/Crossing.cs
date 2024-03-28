using System.Collections.Generic;

namespace Assets.Scripts.Tiles
{
    /// <summary>
    /// Represents a crossing tile.
    /// </summary>
    public class Crossing : Tile
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Crossing"/> class.
        /// </summary>
        public Crossing() => _allowedDirections = new List<bool> { true, true, true, true };
    }
}
