using Assets.Scripts.Tiles;
using System;
using System.Collections.Generic;
using UnityEngine;
using static Assets.Scripts.Tiles.Direction;
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
        private const int _tileSizeX = 16;

        /// <summary>
        /// The size of the tiles in the y-direction.
        /// </summary>
        private const int _tileSizeY = 16;

        /// <summary>
        /// Represents the room objects.
        /// </summary>
        public RoomObjects RoomObjects;

        private readonly Random _random = new Random();

        /// <summary>
        /// This contains the whole 'pipeline' of level generation, including initialising the grid and placing teleporters.
        /// </summary>
        public void Awake()
        {
            int gridWidthX = 10;
            int gridWidthY = 10;

            _grid = new Grid(gridWidthX, gridWidthY);

            _grid.Init();

            int amountOfRooms = 25;

            while (amountOfRooms > 0)
            {
                _grid.PlaceRandomRoom();
                amountOfRooms--;
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

            for (int y = 0; y < _grid.Height; y++)
            {
                for (int x = 0; x < _grid.Width; x++)
                {
                    PlaceTile(x, y, _grid[x, y].Tile);
                }
            }

            JoinConnectedComponentsWithTeleporters();
        }

        /// <summary>
        /// Places a tile at the specified coordinates in the world.
        /// </summary>
        /// <param name="x">The x-coordinates of the room.</param>
        /// <param name="y">The y-coordinates of the room.</param>
        /// <param name="tile">The tile that needs to be placed.</param>
        public void PlaceTile(int x, int y, Tile tile)
        {
            GameObject prefab = tile switch
            {
                Corner => RoomObjects.Corner,
                Crossing => RoomObjects.Crossing,
                DeadEnd => RoomObjects.DeadEnd,
                Empty => RoomObjects.Empty,
                Room => RoomObjects.Room,
                Straight => RoomObjects.Straight,
                TSection => RoomObjects.TSection,
                _ => null
            };

            if (prefab != null)
            {
                tile.GameObject = Instantiate(prefab,
                    new Vector3(x * _tileSizeX, 0, y * _tileSizeY),
                    Quaternion.Euler(0, tile.Facing.RotationDegrees(), 0),
                    transform);
            }
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
        private static Color[] _colors = { Color.red, Color.blue, Color.green, Color.yellow, Color.magenta, Color.cyan };
        private static int _colorIndex = -1;
        private static Color GetUnusedColor() => _colors[_colorIndex = (_colorIndex + 1) % _colors.Length];
    }
}
