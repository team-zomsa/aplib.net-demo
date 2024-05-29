using System.Collections.Generic;

namespace Assets.Scripts.Tiles
{
    /// <summary>
    /// Represents a start version of a <see cref="Room"/> tile.
    /// </summary>
    public class StartRoom : Room
    {
        public StartRoom(List<Direction> connectingDirections, List<Direction> doors) : base(connectingDirections, doors)
        {
        }
    }
}
