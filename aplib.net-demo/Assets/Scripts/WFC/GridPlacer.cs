using Assets.Scripts.Tiles;
using System.Collections.Generic;
using UnityEngine;
using static Assets.Scripts.Tiles.Direction;

namespace Assets.Scripts.WFC
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

        /// <summary>
        /// This contains the whole 'pipeline' of level generation, including initialising the grid and placing teleporters.
        /// </summary>
        public void Awake()
        {
            _grid = new Grid(5, 5);

            _grid.Init();

            TempFillFunction();

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
        /// A temporary function to fill the grid with rooms.
        /// </summary>
        public void TempFillFunction()
        {
            _grid[0, 0].Tile = new TSection(West);
            _grid[0, 1].Tile = new Crossing();
            _grid[0, 2].Tile = new DeadEnd(South);
            _grid[0, 3].Tile = new Straight(East);
            _grid[1, 0].Tile = new TSection();
            _grid[1, 1].Tile = new Straight();
            _grid[1, 2].Tile = new Corner(South);
            _grid[1, 3].Tile = new Crossing();
            _grid[2, 0].Tile = new Corner();
            _grid.PlaceRoom(2, 1, new Room(new List<Direction> { North, East, South, West }));

            _grid.PlaceRoom(3, 3, new Room(new List<Direction> { North, East, South, West }));
            _grid[3, 2].Tile = new Corner(East);
            _grid[4, 2].Tile = new TSection(West);
            _grid[4, 1].Tile = new Straight();
            _grid.PlaceRoom(4, 0, new Room(new List<Direction> { North, East, South, West }));
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
                Corner   => RoomObjects.Corner,
                Crossing => RoomObjects.Crossing,
                DeadEnd  => RoomObjects.DeadEnd,
                Empty    => RoomObjects.Empty,
                Room     => RoomObjects.Room,
                Straight => RoomObjects.Straight,
                TSection => RoomObjects.TSection,
                _        => null
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
            IList<ISet<Cell>> connectedComponents = _grid.DetermineConnectedComponents();

            // We draw all the connected components individually
            foreach (ISet<Cell> connectedComponent in connectedComponents)
            {
                Color color = GetUnusedColor();
                foreach (Cell cell in connectedComponent)
                    cell.Tile.GameObject.GetComponent<MeshRenderer>().material.color = color;
            }

            // TODO the joining of the connected components with teleporters will be done in another PR.
        }

        private static Color[] _colors = { Color.red, Color.blue, Color.green, Color.yellow, Color.magenta, Color.cyan };
        private static int _colorIndex = -1;
        private static Color GetUnusedColor() => _colors[_colorIndex = (_colorIndex + 1) % _colors.Length];
    }
}
