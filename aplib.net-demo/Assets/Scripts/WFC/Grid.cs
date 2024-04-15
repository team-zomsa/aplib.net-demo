using Assets.Scripts.Tiles;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using static Assets.Scripts.Tiles.Direction;

namespace WFC
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
            get => _cells[CoordinatesToIndex(x, y)];
            set => _cells[CoordinatesToIndex(x, y)] = value;
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
                (int x, int y) = IndexToCoordinates(i);
                _cells.Add(new Cell(x, y));
            }
        }

        /// <summary>
        /// Given an index, calculates the coordinates of the belonging tile.
        /// </summary>
        /// <param name="index">Ranging from 0 to infinity, and beyond! But not too far, as you will get an exception.</param>
        /// <returns>The coordinates of the belonging tile.</returns>
        /// <exception cref="IndexOutOfRangeException">
        /// When index is larger than <see cref="Width"/> times /// <see cref="Height"/>.
        /// </exception>
        protected (int x, int y) IndexToCoordinates(int index)
        {
            if (index >= Width * Height) throw new IndexOutOfRangeException("Index specified is out of range.");
            return (index % Width, index / Width);
        }

        /// <summary>
        /// Given coordinates, calculates the index of the belonging tile.
        /// </summary>
        /// <param name="x">The X coordinate of the tile in question.</param>
        /// <param name="y">The Y coordinate of the tile in question.</param>
        /// <returns>The index of the tile in question.</returns>
        /// <exception cref="IndexOutOfRangeException">
        /// When x or y exceed the <see cref="Width"/> or <see cref="Height"/>.
        /// </exception>
        protected int CoordinatesToIndex(int x, int y)
        {
            if (x >= Width || y >= Height) throw new IndexOutOfRangeException("Coordinates specified are out of range.");
            return y * Width + x;
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

        /// <summary>
        /// Given a cell, this method returns all directly adjacent cells, excluding diagonal neighbours.
        /// When the cell is on an edge, it will return less than 4 neighbours.
        /// </summary>
        /// <param name="cell">The centre cell, who's neighbours need to be determined.</param>
        /// <returns>All neighbouring cells, excluding diagonal neighbours.</returns>
        /// <remarks>This does not check if the tiles in the cells are connected.</remarks>
        /// <seealso cref="GetConnectedNeighbours"/>
        public ICollection<Cell> Get4NeighbouringCells(Cell cell)
        {
            ICollection<Cell> neighbours = new Collection<Cell>();
            if (cell.X > 0)          neighbours.Add(this[cell.X - 1, cell.Y]);
            if (cell.X < Width - 1)  neighbours.Add(this[cell.X + 1, cell.Y]);
            if (cell.Y > 0)          neighbours.Add(this[cell.X, cell.Y - 1]);
            if (cell.Y < Height - 1) neighbours.Add(this[cell.X, cell.Y + 1]);
            return neighbours;
        }

        /// <summary>
        /// Given a cell, this method returns all directly adjacent cells, including diagonal neighbours.
        /// When the cell is on an edge, it will return less than 8 neighbours.
        /// </summary>
        /// <param name="cell">The centre cell, who's neighbours need to be determined.</param>
        /// <returns>All neighbouring cells, including diagonal neighbours.</returns>
        /// <remarks>This does not check if the tiles in the cells are connected.</remarks>
        /// <seealso cref="GetConnectedNeighbours"/>
        public ICollection<Cell> Get8NeighbouringCells(Cell cell)
        {
            ICollection<Cell> neighbours = new Collection<Cell>();
            for (int i = -1; i < 2; i++)
            {
                if (cell.X + i < 0 || cell.X + i >= Width) continue; // No out of range
                for (int j = -1; j < 2; j++)
                {
                    if (i == j && i == 0) continue; // Skip the cell itself
                    if (cell.Y + j < 0 || cell.Y + j >= Height) continue; // No out of range
                    neighbours.Add(this[cell.X + i, cell.Y + j]);
                }
            }
            return neighbours;
        }
        
        /// <summary>
        /// Given a cell, this method determines all directly adjacent neighbours which are connected by their <see cref="Tile"/>s.
        /// These are at most 4, as diagonal tiles cannot be connected.
        /// </summary>
        /// <param name="cell">The cell whose connected tiles are to be determined.</param>
        /// <returns>The cells directly connected through their tiles</returns>
        /// <remarks>Assumes that the cells are assigned a tile.</remarks>
        public ICollection<Cell> GetConnectedNeighbours(Cell cell)
        {
            ICollection<Cell> connectedNeighbours = new Collection<Cell>();
            ICollection<Cell> neighbours = Get4NeighbouringCells(cell); // Note: no diagonal neighbours
            foreach (Cell neighbour in neighbours)
            {
                if      (cell.Tile.CanConnectInDirection(East) && neighbour.X > cell.X && neighbour.Tile.CanConnectInDirection(West))
                    connectedNeighbours.Add(neighbour);
                else if (cell.Tile.CanConnectInDirection(West) && neighbour.X < cell.X && neighbour.Tile.CanConnectInDirection(East))
                    connectedNeighbours.Add(neighbour);
                else if (cell.Tile.CanConnectInDirection(North) && neighbour.Y > cell.Y && neighbour.Tile.CanConnectInDirection(South))
                    connectedNeighbours.Add(neighbour);
                else if (cell.Tile.CanConnectInDirection(South) && neighbour.Y < cell.Y && neighbour.Tile.CanConnectInDirection(North))
                    connectedNeighbours.Add(neighbour);
            }

            return connectedNeighbours;
        }

        /// <summary>
        /// This method goes through all cells in this grid, to determine all connected components.
        /// </summary>
        /// <returns>A collection of sets of cells, where the sets of cells represent the connected components.</returns>
        public IList<ISet<Cell>> DetermineConnectedComponents()
        {
            ISet<Cell> unvisitedCells = new HashSet<Cell>(_cells.Where(cell => cell.Tile is not Empty)); // Deep copy
            IList<ISet<Cell>> connectedComponents = new List<ISet<Cell>>();

            while (unvisitedCells.Count > 0)
            {
                ISet<Cell> connectedComponent = new HashSet<Cell>();
                connectedComponents.Add(connectedComponent);

                // Determine connected component, which updates unvisitedCells and connectedComponent
                DetermineSingleConnectedComponent(unvisitedCells, connectedComponent, unvisitedCells.First());
            }

            return connectedComponents;
        }

        /// <summary>
        /// Given a cell, this method determines the connected component to which the cell belongs.
        /// </summary>
        /// <param name="searchSpace">The cells which are considered to be possibly connected to <paramref name="cell"/>.</param>
        /// <param name="connectedComponent">The <see cref="ISet{Cell}"/> in which the connected component is stored.</param>
        /// <param name="cell">The cell to start searching from.</param>
        /// <remarks>The connected component is stored in <paramref name="connectedComponent"/>.</remarks>
        public void DetermineSingleConnectedComponent(in ISet<Cell> searchSpace, in ISet<Cell> connectedComponent, Cell cell)
        {
            connectedComponent.Add(cell);
            searchSpace.Remove(cell);

            ICollection<Cell> connectedNeighbours = GetConnectedNeighbours(cell);
            foreach (Cell connectedNeighbour in connectedNeighbours)
            {
                if (!searchSpace.Contains(connectedNeighbour)) continue; // Already visited

                connectedComponent.Add(connectedNeighbour);
                searchSpace.Remove(connectedNeighbour);

                DetermineSingleConnectedComponent(searchSpace, connectedComponent, connectedNeighbour);
            }
        }
    }
}
