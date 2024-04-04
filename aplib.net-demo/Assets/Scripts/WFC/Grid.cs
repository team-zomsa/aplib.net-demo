using System.Collections.Generic;
using Assets.Scripts.Tiles;

namespace Assets.Scripts.WFC
{
    /// <summary>
    /// Represents a grid.
    /// </summary>
    public class Grid
    {
        /// <summary>
        /// The width of the grid.
        /// </summary>
        public readonly int Width;
        
        /// <summary>
        /// The height of the grid.
        /// </summary>
        public readonly int Height;
        
        /// <summary>
        /// The cells of the grid.
        /// </summary>
        private readonly List<Cell> _cells;

        /// <summary>
        /// Initializes a new instance of the <see cref="Grid"/> class.
        /// </summary>
        /// <param name="width">The width of the grid.</param>
        /// <param name="height">The height of the grid.</param>
        public Grid(int width, int height)
        {
            Width = width;
            Height = height;
            _cells = new List<Cell>();
        }

        /// <summary>
        /// Gets or sets the cell at the specified coordinates.
        /// </summary>
        /// <param name="x">The x-coordinate of the cell.</param>
        /// <param name="y">The y-coordinate of the cell.</param>
        public Cell this[int x, int y]
        {
            get => _cells[y * Width + x];
            set => _cells[y * Width + x] = value;
        }

        /// <summary>
        /// Gets or sets the cell at the specified index.
        /// </summary>
        /// <param name="index">The index of the cell.</param>
        public Cell this[int index]
        {
            get => _cells[index];
            set => _cells[index] = value;
        }

        /// <summary>
        /// Initializes the grid.
        /// </summary>
        public void Init()
        {
            for (int i = 0; i < Width * Height; i++)
            {
                _cells.Add(new Cell());
            }
        }

        /// <summary>
        /// Places a room at the specified coordinates in the grid.
        /// </summary>
        /// <param name="x">The x-coordinates of the room.</param>
        /// <param name="y">The y-coordinates of the room.</param>
        /// <param name="room">The room that needs to be placed.</param>
        public void PlaceRoom(int x, int y, Room room)
        {
            this[x, y].Tile = room;
            this[x, y].Candidates = new List<Tile>();
        }
    }
}
