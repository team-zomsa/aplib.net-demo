using Assets.Scripts.Tiles;
using System.Collections.Generic;
using static Assets.Scripts.Tiles.Direction;

namespace Assets.Scripts.WFC
{
    /// <summary>
    /// Represents a cell in the grid.
    /// </summary>
    public class Cell
    {
        /// <summary>
        /// The tile of the cell.
        /// </summary>
        public Tile Tile { get; set; }

        /// <summary>
        /// The possible tiles that can be placed in this cell.
        /// </summary>
        public List<Tile> Candidates { get; set; }

        /// <summary>
        /// The x coordinate of this cell, within its grid.
        /// </summary>
        public int X { get; }

        /// <summary>
        /// The y coordinate of this cell, within its grid.
        /// </summary>
        public int Y { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Cell"/> class.
        /// </summary>
        /// <param name="posX">The X coordinate of this cell, within its grid.</param>
        /// <param name="posY">The Y coordinate of this cell, within its grid.</param>
        public Cell(int posX, int posY)
        {
            X = posX;
            Y = posY;

            Tile = new Empty();
            Candidates = new List<Tile>()
            {
                new Corner(),
                new Corner(East),
                new Corner(South),
                new Corner(West),
                new Crossing(),
                new DeadEnd(),
                new DeadEnd(East),
                new DeadEnd(South),
                new DeadEnd(West),
                new Empty(),
                new Straight(),
                new Straight(East),
                new TSection(),
                new TSection(East),
                new TSection(South),
                new TSection(West),
            };
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Cell"/> class.
        /// </summary>
        /// <param name="tiles">The possible tiles that can be placed in this cell.</param>
        /// <param name="posX">The X coordinate of this cell, within its grid.</param>
        /// <param name="posY">The Y coordinate of this cell, within its grid.</param>
        public Cell(int posX, int posY, List<Tile> tiles)
        {
            X = posX;
            Y = posY;

            Tile = new Empty();
            Candidates = tiles;
        }
    }
}
