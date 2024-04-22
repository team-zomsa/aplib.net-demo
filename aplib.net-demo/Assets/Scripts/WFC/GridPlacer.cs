using Assets.Scripts.Tiles;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using Random = System.Random;

namespace Assets.Scripts.Wfc
{
    /// <summary>
    /// Represents the grid placer.
    /// </summary>
    public class GridPlacer : MonoBehaviour
    {
        /// <summary>
        /// Represents the grid.
        /// </summary>
        private Grid _grid;

        /// <summary>
        /// The size of the tiles in the x-direction.
        /// </summary>
        public int tileSizeX = 16;

        /// <summary>
        /// The size of the tiles in the y-direction.
        /// </summary>
        [FormerlySerializedAs("tileSizeY")] public int tileSizeZ = 16;

        /// <summary>
        /// Represents the room objects.
        /// </summary>
        public RoomObjects roomObjects;

        // TODO: Move random
        private readonly Random _random = new();

        /// <summary>
        /// The width of the grid in the x-direction.
        /// </summary>
        public int gridWidthX = 10;

        /// <summary>
        /// The width of the grid in the z-direction.
        /// </summary>
        public int gridWidthZ = 10;

        /// <summary>
        /// The amount of rooms that need to be placed.
        /// </summary>
        public int amountOfRooms = 5;

        /// <summary>
        /// This contains the whole 'pipeline' of level generation, including initialising the grid and placing teleporters.
        /// </summary>
        public void Awake()
        {
            MakeScene();
        }

        /// <summary>
        /// Makes the scene.
        /// </summary>
        /// <exception cref="Exception">The amount of rooms is larger than the available places in the grid.</exception>
        public void MakeScene()
        {
            if (amountOfRooms > gridWidthX * gridWidthZ)
            {
                throw new Exception("The amount of rooms is larger than the available places in the grid.");
            }

            MakeGrid();

            PlaceGrid();

            JoinConnectedComponentsWithTeleporters();
        }

        /// <summary>
        /// Makes the grid.
        /// </summary>
        private void MakeGrid()
        {
            _grid = new Grid(gridWidthX, gridWidthZ);

            _grid.Init();

            int noRooms = 0;

            while (noRooms < amountOfRooms)
            {
                _grid.PlaceRandomRoom();
                noRooms++;
            }

            while (!_grid.IsFullyCollapsed())
            {
                List<Cell> lowestEntropyCells = _grid.GetLowestEntropyCells();

                int index = _random.Next(lowestEntropyCells.Count);

                Cell cell = lowestEntropyCells[index];
                cell.Tile = cell.Candidates[_random.Next(cell.Candidates.Count)];
                cell.Candidates.Clear();

                _grid.RemoveUnconnectedNeighbourCandidates(cell);
            }
        }

        /// <summary>
        /// Places the grid in the world.
        /// </summary>
        private void PlaceGrid()
        {
            for (int z = 0; z < _grid.Height; z++)
            {
                for (int x = 0; x < _grid.Width; x++)
                {
                    PlaceTile(x, z, _grid[x, z].Tile);
                }
            }
        }

        /// <summary>
        /// Places a tile at the specified coordinates in the world.
        /// </summary>
        /// <param name="x">The x-coordinates of the room.</param>
        /// <param name="z">The z-coordinates of the room.</param>
        /// <param name="tile">The tile that needs to be placed.</param>
        private void PlaceTile(int x, int z, Tile tile)
        {
            GameObject prefab = tile switch
            {
                Corner => roomObjects.Corner,
                Crossing => roomObjects.Crossing,
                DeadEnd => roomObjects.DeadEnd,
                Room => roomObjects.Room,
                Straight => roomObjects.Straight,
                TSection => roomObjects.TSection,
                _ => roomObjects.Empty
            };

            tile.GameObject = Instantiate(prefab,
                new Vector3(x * tileSizeX, 0, z * tileSizeZ),
                Quaternion.Euler(0, tile.Facing.RotationDegrees(), 0),
                transform);
        }

        /// <summary>
        /// Fist calculate the connected components of the grid, then join them with teleporters.
        /// </summary>
        private void JoinConnectedComponentsWithTeleporters()
        {
            IEnumerable<ISet<Cell>> connectedComponents = _grid.DetermineConnectedComponents();

            // We draw all the connected components individually
            foreach (ISet<Cell> connectedComponent in connectedComponents)
            {
                Color color = GetUnusedColor();
                foreach (Cell cell in connectedComponent)
                    cell.Tile.GameObject.GetComponent<MeshRenderer>().material.color = color;
            }

            // TODO the joining of the connected components with teleporters will be done in another PR.
        }

        // Here are temporary helper methods used to display the connected components in different colors.
        private static readonly Color[] _colors = { Color.red, Color.blue, Color.green, Color.yellow, Color.magenta, Color.cyan };
        private static int _colorIndex = -1;
        private static Color GetUnusedColor() => _colors[_colorIndex = (_colorIndex + 1) % _colors.Length];
    }
}
