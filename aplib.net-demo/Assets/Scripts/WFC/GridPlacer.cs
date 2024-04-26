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
        /// The size of the tiles in the x-direction.
        /// </summary>
        [SerializeField]
        private int _tileSizeX = 16;

        /// <summary>
        /// The size of the tiles in the z-direction.
        /// </summary>
        [SerializeField]
        private int _tileSizeZ = 16;

        /// <summary>
        /// Represents the room objects.
        /// </summary>
        [SerializeField]
        private RoomObjects _roomObjects;

        /// <summary>
        /// Represents the door object.
        /// </summary>
        [SerializeField]
        private GameObject _doorPrefab;

        /// <summary>
        /// A boolean that indicates whether a seed is used.
        /// </summary>
        [SerializeField]
        private bool _useSeed;

        /// <summary>
        /// The seed used for the random number generator.
        /// </summary>
        [SerializeField]
        private int _seed;

        /// <summary>
        /// The width of the grid in the x-direction.
        /// </summary>
        [SerializeField]
        private int _gridWidthX = 10;

        /// <summary>
        /// The width of the grid in the z-direction.
        /// </summary>
        [SerializeField]
        private int _gridWidthZ = 10;

        /// <summary>
        /// The amount of rooms that need to be placed.
        /// </summary>
        [SerializeField]
        private int _amountOfRooms = 5;

        /// <summary>
        /// Represents the grid.
        /// </summary>
        private Grid _grid;

        /// <summary>
        /// The random number generator.
        /// </summary>
        private Random _random = new();

        /// <summary>
        /// This contains the whole 'pipeline' of level generation, including initialising the grid and placing teleporters.
        /// </summary>
        public void Awake() => MakeScene();

        /// <summary>
        /// Makes the scene.
        /// </summary>
        /// <exception cref="Exception">The amount of rooms is larger than the available places in the grid.</exception>
        public void MakeScene()
        {
            if (_amountOfRooms > _gridWidthX * _gridWidthZ)
                throw new Exception("The amount of rooms is larger than the available places in the grid.");

            if (_useSeed) _random = new Random(_seed);

            MakeGrid();

            PlaceGrid();

            JoinConnectedComponentsWithTeleporters();
        }

        /// <summary>
        /// Makes the grid.
        /// </summary>
        private void MakeGrid()
        {
            _grid = new Grid(_gridWidthX, _gridWidthZ, _random);

            _grid.Init();

            int numberOfRooms = 0;

            while (numberOfRooms < _amountOfRooms)
            {
                _grid.PlaceRandomRoom();
                numberOfRooms++;
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
                for (int x = 0; x < _grid.Width; x++) PlaceTile(x, z, _grid[x, z].Tile);
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
            if (tile is Room room) PlaceDoors(x, z, room);

            GameObject prefab = tile switch
            {
                Corner => _roomObjects.Corner,
                Crossing => _roomObjects.Crossing,
                DeadEnd => _roomObjects.DeadEnd,
                Room => _roomObjects.Room,
                Straight => _roomObjects.Straight,
                TSection => _roomObjects.TSection,
                _ => _roomObjects.Empty
            };

            tile.GameObject = Instantiate
            (
                prefab,
                new Vector3(x * _tileSizeX, 0, z * _tileSizeZ),
                Quaternion.Euler(0, tile.Facing.RotationDegrees(), 0),
                transform
            );
        }

        /// <summary>
        /// Place the doors for the given room in the world. Which doors need to be spawned is determined from the
        /// allowed directions of the room.
        /// </summary>
        /// <param name="x">The x-position of the room, in the grid.</param>
        /// <param name="z">The z-position of the room, in the grid.</param>
        /// <param name="room">The room for which the doors need to be spawned.</param>
        // ReSharper disable once SuggestBaseTypeForParameter
        private void PlaceDoors(int x, int z, Room room)
        {
            Vector3 roomPosition = new(x * _tileSizeX, 0, z * _tileSizeZ);
            // Get (half of) the depth of the door model
            float doorDepthExtend = _doorPrefab.GetComponent<Renderer>().bounds.extents.z;
            // Calculate the distance from the room center to where a door should be placed
            float doorDistanceFromRoomCenter = (_tileSizeX / 2f) - doorDepthExtend;

            Quaternion roomRotation = Quaternion.Euler(0, room.Facing.RotationDegrees(), 0);

            foreach (Direction direction in room.ConnectingDirections)
            {
                // # Calculate where the door should be placed

                Vector3 relativeDoorPosition = direction switch
                {
                    North => new Vector3(-doorDistanceFromRoomCenter, 0, 0),
                    East => new Vector3(0, 0, doorDistanceFromRoomCenter),
                    South => new Vector3(doorDistanceFromRoomCenter, 0, 0),
                    West => new Vector3(0, 0, -doorDistanceFromRoomCenter),
                    _ => throw new UnityException("Invalid direction when placing door")
                };
                Vector3 doorPosition = roomPosition + relativeDoorPosition;

                // # Calculate the rotation the door should have

                // The `RotateLeft` here is because the rotation of the grid and the rotation of the door model do not
                // line up
                Quaternion relativeDoorRotation = Quaternion.Euler(0, direction.RotateLeft().RotationDegrees(), 0);
                Quaternion doorRotation = roomRotation * relativeDoorRotation;

                // # Spawn the door

                _ = Instantiate(_doorPrefab, doorPosition, doorRotation, transform);
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
        private static readonly Color[] _colors =
        {
            Color.red, Color.blue, Color.green, Color.yellow, Color.magenta, Color.cyan
        };

        private static int _colorIndex = -1;
        private static Color GetUnusedColor() => _colors[_colorIndex = (_colorIndex + 1) % _colors.Length];
    }
}
