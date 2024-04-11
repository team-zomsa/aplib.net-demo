using System.Collections.Generic;

namespace Assets.Scripts.Tiles
{
    /// <summary>
    /// Represents a crossing tile.
    /// ___ ___
    /// |_| |_|
    /// ___ ___
    /// |_| |_|
    /// </summary>
    public class Crossing : Tile
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Crossing"/> class.
        /// </summary>
        public Crossing() => AllowedDirections = new List<bool> { true, true, true, true };
    }
}
