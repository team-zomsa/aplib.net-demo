// This program has been developed by students from the bachelor Computer Science at Utrecht
// University within the Software Project course.
// Copyright Utrecht University (Department of Information and Computing Sciences)

using System.Collections.Generic;

namespace Assets.Scripts.Tiles
{
    /// <summary>
    /// Represents an empty tile.
    /// <para/>
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
