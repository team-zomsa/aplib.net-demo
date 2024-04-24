﻿using Assets.Scripts.Tiles;
using System.Collections.Generic;
using UnityEngine;
using static Assets.Scripts.Tiles.Direction;

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
        private const int _tileSizeX = 16;

        /// <summary>
        /// The size of the tiles in the y-direction.
        /// </summary>
        private const int _tileSizeY = 16;

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
        /// Represents the grid.
        /// </summary>
        private Grid _grid;

        /// <summary>
        /// This contains the whole 'pipeline' of level generation, including initialising the grid and placing teleporters.
        /// </summary>
        public void Awake()
        {
            _grid = new Grid(5, 5);

            _grid.Init();

            TempFillFunction();

            for (int y = 0; y < _grid.Height; y++)
                for (int x = 0; x < _grid.Width; x++)
                    PlaceTile(x, y, _grid[x, y].Tile);

            JoinConnectedComponentsWithTeleporters();
        }

        /// <summary>
        /// A temporary function to fill the grid with rooms.
        /// </summary>
        private void TempFillFunction()
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
        /// <exception cref="UnityException">Thrown when the <paramref name="tile" /> is of an unkown type.</exception>
        private void PlaceTile(int x, int y, Tile tile)
        {
            if (tile is Room room) PlaceDoors(x, y, room);

            GameObject prefab = tile switch
            {
                Corner => _roomObjects.Corner,
                Crossing => _roomObjects.Crossing,
                DeadEnd => _roomObjects.DeadEnd,
                Empty => _roomObjects.Empty,
                Room => _roomObjects.Room,
                Straight => _roomObjects.Straight,
                TSection => _roomObjects.TSection,
                _ => throw new UnityException("Unknown tile type when placing tile")
            };

            tile.GameObject = Instantiate
            (
                prefab,
                new Vector3(x * _tileSizeX, 0, y * _tileSizeY),
                Quaternion.Euler(0, tile.Facing.RotationDegrees(), 0),
                transform
            );
        }

        /// <summary>
        /// Place the doors for the given room in the world. Which doors need to be spawned is determined from the
        /// allowed directions of the room.
        /// </summary>
        /// <param name="x">The x-position of the room, in the grid.</param>
        /// <param name="y">The y-position of the room, in the grid.</param>
        /// <param name="room">The room for which the doors need to be spawned.</param>
        // ReSharper disable once SuggestBaseTypeForParameter
        private void PlaceDoors(int x, int y, Room room)
        {
            foreach (Direction direction in room.ConnectingDirections)
            {
                // # Calculate where the door should be placed

                Vector3 roomPosition = new(x * _tileSizeX, 0, y * _tileSizeY);
                // Get (half of) the width of the door model
                float doorDepthExtend = _doorPrefab.GetComponent<Renderer>().bounds.extents.z;
                // Calculate the distance from the room center to where the door should be placed
                float doorDistanceFromRoomCenter = (_tileSizeX / 2f) - doorDepthExtend;
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

                Quaternion roomRotation = Quaternion.Euler(0, room.Facing.RotationDegrees(), 0);
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
