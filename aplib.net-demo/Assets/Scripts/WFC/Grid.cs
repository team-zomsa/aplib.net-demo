using Assets.Scripts.Tiles;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using static Assets.Scripts.Tiles.Direction;
using Random = System.Random;

namespace Assets.Scripts.Wfc
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
        /// The random number generator.
        /// </summary>
        private readonly Random _random;

        /// <summary>
        /// Initializes a new instance of the <see cref="Grid"/> class.
        /// </summary>
        /// <param name="width">The width of the grid.</param>
        /// <param name="height">The height of the grid.</param>
        /// <param name="random">The random number generator.</param>
        public Grid(int width, int height, Random random)
        {
            Width = width;
            Height = height;
            _random = random;
            _cells = new List<Cell>();
        }

        /// <summary>
        /// Gets or sets the cell at the specified coordinates.
        /// </summary>
        /// <param name="x">The x-coordinate of the cell.</param>
        /// <param name="z">The z-coordinate of the cell.</param>
        public Cell this[int x, int z]
        {
            get => _cells[CoordinatesToIndex(x, z)];
            set => _cells[CoordinatesToIndex(x, z)] = value;
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
                (int x, int z) = IndexToCoordinates(i);
                _cells.Add(new Cell(x, z));
            }

            for (int x = 0; x < Width; x++)
            {
                this[x, 0].Candidates.RemoveAll(tile => tile.CanConnectInDirection(South));
                this[x, Height - 1].Candidates.RemoveAll(tile => tile.CanConnectInDirection(North));
            }

            for (int z = 0; z < Height; z++)
            {
                this[0, z].Candidates.RemoveAll(tile => tile.CanConnectInDirection(West));
                this[Width - 1, z].Candidates.RemoveAll(tile => tile.CanConnectInDirection(East));
            }
        }

        /// <summary>
        /// Given an index, calculates the coordinates of the belonging tile.
        /// </summary>
        /// <param name="index">Ranging from 0 to infinity, and beyond! But not too far, as you will get an exception.</param>
        /// <returns>The coordinates of the belonging tile.</returns>
        /// <exception cref="IndexOutOfRangeException">
        /// When index is larger than <see cref="Width"/> times <see cref="Height"/>.
        /// </exception>
        private (int x, int z) IndexToCoordinates(int index)
        {
            if (index >= Width * Height) throw new IndexOutOfRangeException("Index specified is out of range.");
            return (index % Width, index / Width);
        }

        /// <summary>
        /// Given coordinates, calculates the index of the belonging tile.
        /// </summary>
        /// <param name="x">The X coordinate of the tile in question.</param>
        /// <param name="z">The Z coordinate of the tile in question.</param>
        /// <returns>The index of the tile in question.</returns>
        /// <exception cref="IndexOutOfRangeException">
        /// When x or z exceed the <see cref="Width"/> or <see cref="Height"/>.
        /// </exception>
        private int CoordinatesToIndex(int x, int z)
        {
            if (x >= Width || z >= Height) throw new IndexOutOfRangeException("Coordinates specified are out of range.");
            return z * Width + x;
        }

        /// <summary>
        /// Places a room at the specified coordinates in the grid.
        /// </summary>
        /// <param name="x">The x-coordinates of the room.</param>
        /// <param name="z">The z-coordinates of the room.</param>
        /// <param name="room">The room that needs to be placed.</param>
        public void PlaceRoom(int x, int z, Room room)
        {
            this[x, z].Tile = room;
            this[x, z].Candidates = new List<Tile>();

            RemoveUnconnectedNeighbourCandidates(this[x, z]);
        }

        /// <summary>
        /// Places a room at random coordinates in the grid.
        /// </summary>
        public void PlaceRandomRoom()
        {
            List<Cell> notFinished = _cells.FindAll(cell => cell.Candidates.Count > 0);

            int index = _random.Next(notFinished.Count);
            Cell cell = notFinished[index];

            List<Direction> directions = new() { North, East, South, West };

            if (cell.X == 0) directions.Remove(North);
            else if (cell.X == Width - 1) directions.Remove(South);

            if (cell.Z == 0) directions.Remove(West);
            else if (cell.Z == Height - 1) directions.Remove(East);

            PlaceRoom(cell.X, cell.Z, new Room(directions));

            RemoveUnconnectedNeighbourCandidates(cell);
        }

        /// <summary>
        /// Given a cell, this method returns all directly adjacent cells, excluding diagonal neighbours.
        /// When the cell is on an edge, it will return less than 4 neighbours.
        /// </summary>
        /// <param name="cell">The centre cell, who's neighbours need to be determined.</param>
        /// <returns>All neighbouring cells, excluding diagonal neighbours.</returns>
        /// <remarks>This does not check if the tiles in the cells are connected.</remarks>
        /// <seealso cref="GetConnectedNeighbours"/>
        private IEnumerable<Cell> Get4NeighbouringCells(Cell cell)
        {
            ICollection<Cell> neighbours = new Collection<Cell>();
            if (cell.X > 0) neighbours.Add(this[cell.X - 1, cell.Z]);
            if (cell.X < Width - 1) neighbours.Add(this[cell.X + 1, cell.Z]);
            if (cell.Z > 0) neighbours.Add(this[cell.X, cell.Z - 1]);
            if (cell.Z < Height - 1) neighbours.Add(this[cell.X, cell.Z + 1]);
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
                if (cell.X + i < 0 || cell.X + i >= Width) continue; // Not out of range
                for (int j = -1; j < 2; j++)
                {
                    if (i == j && i == 0) continue; // Skip the cell itself
                    if (cell.Z + j < 0 || cell.Z + j >= Height) continue; // Not out of range
                    neighbours.Add(this[cell.X + i, cell.Z + j]);
                }
            }
            return neighbours;
        }

        /// <summary>
        /// Given a cell, this method determines all directly adjacent neighbours which are connected by their
        /// <see cref="Tile"/>s. These are at most 4, as diagonal tiles cannot be connected.
        /// </summary>
        /// <param name="cell">The cell whose connected tiles are to be determined.</param>
        /// <returns>The cells directly connected through their tiles</returns>
        /// <remarks>Assumes that the cells are assigned a tile.</remarks>
        private IEnumerable<Cell> GetConnectedNeighbours(Cell cell)
        {
            ICollection<Cell> connectedNeighbours = new Collection<Cell>();
            IEnumerable<Cell> neighbours = Get4NeighbouringCells(cell); // Note: no diagonal neighbours
            foreach (Cell neighbour in neighbours)
            {
                if (cell.Tile.CanConnectInDirection(East) && neighbour.X > cell.X && neighbour.Tile.CanConnectInDirection(West)
                    || cell.Tile.CanConnectInDirection(West) && neighbour.X < cell.X && neighbour.Tile.CanConnectInDirection(East)
                    || cell.Tile.CanConnectInDirection(North) && neighbour.Z > cell.Z && neighbour.Tile.CanConnectInDirection(South)
                    || cell.Tile.CanConnectInDirection(South) && neighbour.Z < cell.Z && neighbour.Tile.CanConnectInDirection(North))
                    connectedNeighbours.Add(neighbour);
            }

            return connectedNeighbours;
        }

        /// <summary>
        /// This method goes through all cells in this grid, to determine all connected components.
        /// </summary>
        /// <returns>A collection of sets of cells, where the sets of cells represent the connected components.</returns>
        public IEnumerable<ISet<Cell>> DetermineConnectedComponents()
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
        private void DetermineSingleConnectedComponent(in ISet<Cell> searchSpace, in ISet<Cell> connectedComponent, Cell cell)
        {
            connectedComponent.Add(cell);
            searchSpace.Remove(cell);

            IEnumerable<Cell> connectedNeighbours = GetConnectedNeighbours(cell);
            foreach (Cell connectedNeighbour in connectedNeighbours)
            {
                if (!searchSpace.Contains(connectedNeighbour)) continue; // Already visited

                connectedComponent.Add(connectedNeighbour);
                searchSpace.Remove(connectedNeighbour);

                DetermineSingleConnectedComponent(searchSpace, connectedComponent, connectedNeighbour);
            }
        }

        /// <summary>
        /// Removes all candidates from the neighbours of the given cell, which cannot be connected to the cell.
        /// </summary>
        /// <param name="cell">The cell whose neighbours need to be updated.</param>
        public void RemoveUnconnectedNeighbourCandidates(Cell cell)
        {
            IEnumerable<Cell> neighbours = Get4NeighbouringCells(cell);

            foreach (Cell neighbour in neighbours)
            {
                neighbour.Candidates.RemoveAll(tile =>
                    !cell.Tile.CanConnectInDirection(East) && neighbour.X > cell.X && tile.CanConnectInDirection(West)
                        || cell.Tile.CanConnectInDirection(East) && neighbour.X > cell.X && !tile.CanConnectInDirection(West)
                        || !cell.Tile.CanConnectInDirection(West) && neighbour.X < cell.X && tile.CanConnectInDirection(East)
                        || cell.Tile.CanConnectInDirection(West) && neighbour.X < cell.X && !tile.CanConnectInDirection(East)
                        || !cell.Tile.CanConnectInDirection(North) && neighbour.Z > cell.Z && tile.CanConnectInDirection(South)
                        || cell.Tile.CanConnectInDirection(North) && neighbour.Z > cell.Z && !tile.CanConnectInDirection(South)
                        || !cell.Tile.CanConnectInDirection(South) && neighbour.Z < cell.Z && tile.CanConnectInDirection(North)
                        || cell.Tile.CanConnectInDirection(South) && neighbour.Z < cell.Z && !tile.CanConnectInDirection(North));
            }
        }

        /// <summary>
        /// Gets the cells with the lowest entropy.
        /// </summary>
        /// <returns>Returns the cells with the lowest entropy</returns>
        public List<Cell> GetLowestEntropyCells()
        {
            List<Cell> notFinished = _cells.FindAll(cell => cell.Candidates.Count > 0);
            int lowestEntropy = notFinished.Min(cell => cell.Entropy);
            return notFinished.FindAll(cell => cell.Entropy == lowestEntropy);
        }

        /// <summary>
        /// Checks if the grid is fully collapsed.
        /// </summary>
        /// <returns>Returns true if the grid is fully collapsed, false otherwise.</returns>
        public bool IsFullyCollapsed() => _cells.TrueForAll(cell => cell.Candidates.Count == 0);
    }
}
