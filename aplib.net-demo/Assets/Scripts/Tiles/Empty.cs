using System.Collections.Generic;

namespace Assets.Scripts.Tiles
{
    /// <summary>
    /// Represents an empty tile.
    /// <br/><br/>
    /// Default orientation (north):
    /// <code>
    ///
    ///  front
    ///    ↑
    /// _______
    /// |     |
    /// |     |
    /// |_____|
    /// </code>
    /// </summary>
    public class Empty : Tile
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Empty"/> class.
        /// </summary>
        public Empty() => ConnectingDirections = new List<Direction>(/* empty */);
    }
}
