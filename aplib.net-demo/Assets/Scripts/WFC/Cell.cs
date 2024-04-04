using Assets.Scripts.Tiles;
using System.Collections.Generic;

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
        /// Initializes a new instance of the <see cref="Cell"/> class.
        /// </summary>
        public Cell()
        {
            Tile = new Empty();
            Candidates = new List<Tile>()
            {
                new Corner(),
                new Corner(1),
                new Corner(2),
                new Corner(3),
                new Crossing(),
                new DeadEnd(),
                new DeadEnd(1),
                new DeadEnd(2),
                new DeadEnd(3),
                new Empty(),
                new Straight(),
                new Straight(1),
                new TSection(),
                new TSection(1),
                new TSection(2),
                new TSection(3),
            };
        }
        
        /// <summary>
        /// Initializes a new instance of the <see cref="Cell"/> class.
        /// </summary>
        /// <param name="tiles">The possible tiles that can be placed in this cell.</param>
        public Cell(List<Tile> tiles)
        {
            Tile = new Empty();
            Candidates = tiles;
        }
    }
}
