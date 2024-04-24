using Assets.Scripts.Tiles;
using System.Collections.Generic;
using static Assets.Scripts.Tiles.Direction;

namespace Assets.Scripts.Wfc
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
        public int Z { get; }

        /// <summary>
        /// Gets the entropy of the cell.
        /// </summary>
        public int Entropy => Candidates.Count;

        /// <summary>
        /// Initializes a new instance of the <see cref="Cell"/> class.
        /// </summary>
        /// <param name="posX">The X coordinate of this cell, within its grid.</param>
        /// <param name="posZ">The Z coordinate of this cell, within its grid.</param>
        public Cell(int posX, int posZ)
        {
            X = posX;
            Z = posZ;

            Tile = new Empty();
            Candidates = new List<Tile>()
            {
                new Corner(North),
                new Corner(East),
                new Corner(South),
                new Corner(West),
                new Crossing(),
                new DeadEnd(North),
                new DeadEnd(East),
                new DeadEnd(South),
                new DeadEnd(West),
                new Empty(),
                new Straight(North),
                new Straight(East),
                new TSection(North),
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
        /// <param name="posZ">The Z coordinate of this cell, within its grid.</param>
        public Cell(int posX, int posZ, List<Tile> tiles)
        {
            X = posX;
            Z = posZ;

            Tile = new Empty();
            Candidates = tiles;
        }
    }
}
