using Assets.Scripts.Tiles;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using ThreadSafeRandom;
using static Assets.Scripts.Tiles.Direction;

namespace Assets.Scripts.Wfc
{
    /// <summary>
    /// Represents a grid.
    /// </summary>
    public class Grid
    {
        /// <summary>
        /// The cells of the grid.
        /// </summary>
        private readonly List<Cell> _cells;

        /// <summary>
        /// The height of the grid.
        /// </summary>
        public readonly int Height;

        /// <summary>
        /// The width of the grid.
        /// </summary>
        public readonly int Width;

        /// <summary>
        /// Initializes a new instance of the <see cref="Grid" /> class.
        /// </summary>
        /// <param name="width">The width of the grid.</param>
        /// <param name="height">The height of the grid.</param>
        /// <param name="random">The random number generator.</param>
        public Grid(int width, int height)
        {
            Width = width;
            Height = height;
            _cells = new List<Cell>();
        }

        /// <summary>
        /// Gets the cell at the specified coordinates.
        /// </summary>
        /// <param name="x">The x-coordinate of the cell.</param>
        /// <param name="z">The z-coordinate of the cell.</param>
        public Cell this[int x, int z] => _cells[CoordinatesToIndex(x, z)];

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
        /// When index is larger than <see cref="Width" /> times <see cref="Height" />.
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
        /// When x or z exceed the <see cref="Width" /> or <see cref="Height" />.
        /// </exception>
        private int CoordinatesToIndex(int x, int z)
        {
            if (x >= Width || z >= Height)
                throw new IndexOutOfRangeException("Coordinates specified are out of range.");

            return z * Width + x;
        }

        /// <summary>
        /// Places a room at the specified coordinates in the grid.
        /// </summary>
        /// <param name="x">The x-coordinates of the room.</param>
        /// <param name="z">The z-coordinates of the room.</param>
        /// <param name="room">The room that needs to be placed.</param>
        private void PlaceRoom(int x, int z, Room room)
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

            int index = SharedRandom.Next(notFinished.Count);
            Cell cell = notFinished[index];

            List<Direction> directions = new() { North, East, South, West };

            if (cell.X == 0) directions.Remove(West);
            else if (cell.X == Width - 1) directions.Remove(East);

            if (cell.Z == 0) directions.Remove(South);
            else if (cell.Z == Height - 1) directions.Remove(North);

            List<Direction> doors = directions.ToList(); // Temporary deep copy of directions

            PlaceRoom(cell.X, cell.Z, new Room(directions, doors));

            RemoveUnconnectedNeighbourCandidates(cell);
        }

        /// <summary>
        /// Given a cell, this method returns all directly adjacent cells, excluding diagonal neighbours.
        /// When the cell is on an edge, it will return less than 4 neighbours.
        /// </summary>
        /// <param name="cell">The centre cell, who's neighbours need to be determined.</param>
        /// <returns>All neighbouring cells, excluding diagonal neighbours.</returns>
        /// <remarks>This does not check if the tiles in the cells are connected.</remarks>
        /// <seealso cref="GetConnectedNeighbours" />
        public IEnumerable<Cell> Get4NeighbouringCells(Cell cell)
        {
            ICollection<Cell> neighbours = new Collection<Cell>();
            if (cell.X > 0) neighbours.Add(this[cell.X - 1, cell.Z]);
            if (cell.X < Width - 1) neighbours.Add(this[cell.X + 1, cell.Z]);
            if (cell.Z > 0) neighbours.Add(this[cell.X, cell.Z - 1]);
            if (cell.Z < Height - 1) neighbours.Add(this[cell.X, cell.Z + 1]);
            return neighbours;
        }

        /// <summary>
        /// Check if two cells are connected.
        /// </summary>
        /// <param name="cell">The first cell.</param>
        /// <param name="neighbour">The second cell.</param>
        /// <returns>True if the two cells are connected, otherwise false.</returns>
        private static bool IsConnected(Cell cell, Cell neighbour) => cell.Tile.CanConnectInDirection(East) &&
                                                                      neighbour.X > cell.X &&
                                                                      neighbour.Tile.CanConnectInDirection(West)
                                                                      || cell.Tile.CanConnectInDirection(West) &&
                                                                      neighbour.X < cell.X &&
                                                                      neighbour.Tile.CanConnectInDirection(East)
                                                                      || cell.Tile.CanConnectInDirection(North) &&
                                                                      neighbour.Z > cell.Z &&
                                                                      neighbour.Tile.CanConnectInDirection(South)
                                                                      || cell.Tile.CanConnectInDirection(South) &&
                                                                      neighbour.Z < cell.Z &&
                                                                      neighbour.Tile.CanConnectInDirection(North);

        /// <summary>
        /// Given a cell, this method determines all directly adjacent neighbours which are connected by their
        /// <see cref="Tile" />s. These are at most 4, as diagonal tiles cannot be connected.
        /// </summary>
        /// <param name="cell">The cell whose connected tiles are to be determined.</param>
        /// <returns>The cells directly connected through their tiles</returns>
        /// <remarks>Assumes that the cells are assigned a tile.</remarks>
        private IEnumerable<Cell> GetConnectedNeighbours(Cell cell)
        {
            ICollection<Cell> connectedNeighbours = new Collection<Cell>();
            IEnumerable<Cell> neighbours = Get4NeighbouringCells(cell); // Note: no diagonal neighbours
            foreach (Cell neighbour in neighbours)
                if (IsConnected(cell, neighbour))
                    connectedNeighbours.Add(neighbour);

            return connectedNeighbours;
        }

        /// <summary>
        /// Given a cell and a neighbour, this method determines the direction in which the neighbour is connected to the cell.
        /// </summary>
        /// <param name="cell">The cell.</param>
        /// <param name="neighbour">The neighbouring cell.</param>
        /// <returns>The direction in which the neighbour is connected to the cell, or null if they are not connected.</returns>
        public static Direction? GetDirection(Cell cell, Cell neighbour)
        {
            if (cell.Tile.CanConnectInDirection(East) && neighbour.X > cell.X &&
                neighbour.Tile.CanConnectInDirection(West))
                return East;

            if (cell.Tile.CanConnectInDirection(West) && neighbour.X < cell.X &&
                neighbour.Tile.CanConnectInDirection(East))
                return West;

            if (cell.Tile.CanConnectInDirection(North) && neighbour.Z > cell.Z &&
                neighbour.Tile.CanConnectInDirection(South))
                return North;

            if (cell.Tile.CanConnectInDirection(South) && neighbour.Z < cell.Z &&
                neighbour.Tile.CanConnectInDirection(North))
                return South;

            return null;
        }

        /// <summary>
        /// Given a cell, this method determines all directly adjacent neighbours which are connected by their
        /// <see cref="Tile" />.
        /// </summary>
        /// <param name="cell">The cell whose connected tiles are to be determined.</param>
        /// <returns>The cells directly connected through their tiles.</returns>
        private (IEnumerable<Cell> connectedNeighbours, IEnumerable<Cell> neighbouringRooms)
            GetConnectedNeighboursWithoutDoors(Cell cell)
        {
            ICollection<Cell> connectedNeighbours = new Collection<Cell>();
            ICollection<Cell> neighbouringRooms = new Collection<Cell>();
            IEnumerable<Cell> neighbours = Get4NeighbouringCells(cell);

            foreach (Cell neighbour in neighbours)
            {
                Direction? direction = GetDirection(cell, neighbour);

                if (direction is null) continue; // Cells are not connected

                Room cellRoom = cell.Tile as Room;
                Room neighbourRoom = neighbour.Tile as Room;

                bool cellHasDoor = cellRoom?.DoorInDirection(direction.Value) ?? false;
                bool neighbourHasDoor = neighbourRoom?.DoorInDirection(direction.Value.Opposite()) ?? false;

                if (!cellHasDoor && !neighbourHasDoor)
                    connectedNeighbours.Add(neighbour);
                else
                    neighbouringRooms.Add(neighbour);
            }

            return (connectedNeighbours, neighbouringRooms);
        }

        /// <summary>
        /// This method goes through all cells in this grid, to determine all connected components between doors.
        /// </summary>
        /// <returns>A collection of sets of cells, where the sets of cells represent the connected components.</returns>
        public List<(ISet<Cell>, ISet<Cell>)> DetermineConnectedComponentsBetweenDoors()
        {
            ISet<Cell> unvisitedCells = new HashSet<Cell>(_cells.Where(cell => cell.Tile is not Empty));
            List<(ISet<Cell>, ISet<Cell>)> connectedComponents = new();

            while (unvisitedCells.Count > 0)
            {
                ISet<Cell> connectedComponent = new HashSet<Cell>();
                ISet<Cell> neighbouringRooms = new HashSet<Cell>();
                connectedComponents.Add((connectedComponent, neighbouringRooms));

                // Determine connected component, which updates unvisitedCells and connectedComponent
                DetermineSingleConnectedComponentBetweenDoors(unvisitedCells, connectedComponent, neighbouringRooms,
                    unvisitedCells.First());
            }

            return connectedComponents;
        }

        /// <summary>
        /// Given a cell, this method determines the connected component to which the cell belongs.
        /// </summary>
        /// <param name="searchSpace">The cells which are considered to be possibly connected to <paramref name="cell" />.</param>
        /// <param name="connectedComponent">The <see cref="ISet{Cell}" /> in which the connected component is stored.</param>
        /// <param name="neighbouringRooms">The <see cref="ISet{Cell}" /> in which the neighbouring rooms are stored.</param>
        /// <param name="cell">The cell to start searching from.</param>
        private void DetermineSingleConnectedComponentBetweenDoors(in ISet<Cell> searchSpace,
            in ISet<Cell> connectedComponent, in ISet<Cell> neighbouringRooms, Cell cell)
        {
            connectedComponent.Add(cell);
            searchSpace.Remove(cell);

            (IEnumerable<Cell> connectedNeighbours, IEnumerable<Cell> rooms) = GetConnectedNeighboursWithoutDoors(cell);

            neighbouringRooms.UnionWith(rooms);

            foreach (Cell connectedNeighbour in connectedNeighbours)
            {
                if (!searchSpace.Contains(connectedNeighbour)) continue; // Already visited

                connectedComponent.Add(connectedNeighbour);
                searchSpace.Remove(connectedNeighbour);

                DetermineSingleConnectedComponentBetweenDoors(searchSpace, connectedComponent, neighbouringRooms,
                    connectedNeighbour);
            }
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
        /// <param name="searchSpace">The cells which are considered to be possibly connected to <paramref name="cell" />.</param>
        /// <param name="connectedComponent">The <see cref="ISet{Cell}" /> in which the connected component is stored.</param>
        /// <param name="cell">The cell to start searching from.</param>
        /// <remarks>The connected component is stored in <paramref name="connectedComponent" />.</remarks>
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
                neighbour.Candidates.RemoveAll(tile =>
                    !cell.Tile.CanConnectInDirection(East) && neighbour.X > cell.X && tile.CanConnectInDirection(West)
                    || cell.Tile.CanConnectInDirection(East) && neighbour.X > cell.X &&
                    !tile.CanConnectInDirection(West)
                    || !cell.Tile.CanConnectInDirection(West) && neighbour.X < cell.X &&
                    tile.CanConnectInDirection(East)
                    || cell.Tile.CanConnectInDirection(West) && neighbour.X < cell.X &&
                    !tile.CanConnectInDirection(East)
                    || !cell.Tile.CanConnectInDirection(North) && neighbour.Z > cell.Z &&
                    tile.CanConnectInDirection(South)
                    || cell.Tile.CanConnectInDirection(North) && neighbour.Z > cell.Z &&
                    !tile.CanConnectInDirection(South)
                    || !cell.Tile.CanConnectInDirection(South) && neighbour.Z < cell.Z &&
                    tile.CanConnectInDirection(North)
                    || cell.Tile.CanConnectInDirection(South) && neighbour.Z < cell.Z &&
                    !tile.CanConnectInDirection(North));
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

        /// <summary>
        /// Gets a random filled cell.
        /// </summary>
        /// <returns>The random selected filled cell.</returns>
        public Cell GetRandomFilledCell()
        {
            List<Cell> filledCells = _cells.FindAll(cell => cell.Tile is not Empty);
            return filledCells[SharedRandom.Next(filledCells.Count)];
        }

        /// <summary>
        /// Gets all cells that are not empty and do not contain items.
        /// </summary>
        /// <returns>Returns all cells that are not empty and do not contain items.</returns>
        public List<Cell> GetAllCellsNotContainingItems() => _cells.FindAll(cell => cell.Tile is not Empty && !cell.CannotAddItem);

        /// <summary>
        /// Gets all cells that are not empty.
        /// </summary>
        /// <returns>Returns all cells that are not empty.</returns>
        public List<Cell> GetAllNotEmptyTiles() => _cells.FindAll(cell => cell.Tile is not Empty);
    }
}
