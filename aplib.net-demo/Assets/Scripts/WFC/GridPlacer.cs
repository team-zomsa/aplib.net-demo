using Assets.Scripts.Tiles;
using System;
using System.Collections.Generic;
using System.Linq;
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

        [SerializeField]
        private GameObject _teleporterPrefab;

        /// <summary>
        /// Represents the grid.
        /// </summary>
        private Grid _grid;

        /// <summary>
        /// The random number generator.
        /// </summary>
        private Random _random = new();

        /// <summary>
        /// The `Renderer` component for the door prefab.
        /// </summary>
        /// <remarks>Getting a reference to the component is expensive, so we only want to do it once.</remarks>
        private Renderer _doorRenderer;

        /// <summary>
        /// This contains the whole 'pipeline' of level generation, including initialising the grid and placing teleporters.
        /// </summary>
        public void Awake()
        {
            _doorRenderer = _doorPrefab.GetComponent<Renderer>();
            MakeScene();
        }

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
            float doorDepthExtend = _doorRenderer.bounds.extents.z;
            // Calculate the distance from the room center to where a door should be placed
            float doorDistanceFromRoomCenter = (_tileSizeX / 2f) - doorDepthExtend;

            Quaternion roomRotation = Quaternion.Euler(0, room.Facing.RotationDegrees(), 0);

            foreach (Direction direction in room.ConnectingDirections)
            {
                // # Calculate where the door should be placed

                // North is in the negative x direction
                Vector3 relativeDoorPosition = direction switch
                {
                    North => new Vector3(0, 0, doorDistanceFromRoomCenter),
                    East => new Vector3(doorDistanceFromRoomCenter, 0, 0),
                    South => new Vector3(0, 0, -doorDistanceFromRoomCenter),
                    West => new Vector3(-doorDistanceFromRoomCenter, 0, 0),
                    _ => throw new UnityException("Invalid direction when placing door")
                };
                Vector3 doorPosition = roomPosition + relativeDoorPosition;

                // # Calculate the rotation the door should have

                // The `RotateLeft` here is because the rotation of the grid and the rotation of the door model do not
                // line up
                Quaternion relativeDoorRotation = Quaternion.Euler(0, direction.RotationDegrees(), 0);
                Quaternion doorRotation = roomRotation * relativeDoorRotation;

                // # Spawn the door

                _ = Instantiate(_doorPrefab, doorPosition, doorRotation, transform);
            }
        }

        /// <summary>
        /// First calculate the connected components of the grid, then join them with teleporters.
        /// </summary>
        private void JoinConnectedComponentsWithTeleporters()
        {
            IList<ISet<Cell>> connectedComponents = _grid.DetermineConnectedComponents().ToList();

            for (int i = 0; i < connectedComponents.Count - 1; i++)
                for (int j = i + 1; j < connectedComponents.Count; j++) // Binomial product of connected components
                    ConnectConnectedComponentsWithTeleporters(connectedComponents[i], connectedComponents[j]);
        }

        /// <summary>
        /// Given two connected components, this method will connect these two <i><b>even more</b></i>.
        /// It will place a teleporter between them, making navigation in both directions possible.
        /// </summary>
        /// <param name="connectedComponent1">Arbitrary connected component 1</param>
        /// <param name="connectedComponent2">Arbitrary connected component 2, but not different from <paramref name="connectedComponent1"/>.</param>
        private void ConnectConnectedComponentsWithTeleporters(IEnumerable<Cell> connectedComponent1, IEnumerable<Cell> connectedComponent2)
        {
            // if (connectedComponent1.Count < 2 || connectedComponent2.Count < 2) TODO
            // {
            //     throw new NotSupportedException(
            //         "A connected component consisting of a single cell can not be ensured to have one telepoter.");
            // }

            // Determine which cells to connect
            Cell cell1 = connectedComponent1.First();
            Cell cell2 = connectedComponent2.Last(); // Last as opposed to first, to ensure that a single room only has one portal

            ConnectCellsWithTeleporters(cell1, cell2);
        }

        private void ConnectCellsWithTeleporters(Cell a, Cell b)
        {
            Teleporter.Teleporter teleporterA = PlaceTeleporter(CentreOfCell(a));
            Teleporter.Teleporter teleporterB = PlaceTeleporter(CentreOfCell(b));

            teleporterA.targetTeleporter = teleporterB;
            teleporterB.targetTeleporter = teleporterA;
        }

        private Teleporter.Teleporter PlaceTeleporter(Vector3 coordinates)
        {
            // Give all teleporters a parent object for organization
            GameObject teleportersParent = GameObject.Find("Teleporters");
            if (teleportersParent == null)
                teleportersParent = new GameObject("Teleporters");

            return Instantiate(_teleporterPrefab, coordinates, Quaternion.identity, teleportersParent.transform)
                .GetComponent<Teleporter.Teleporter>();
        }

        public Vector3 CentreOfCell(Cell cell) => new((cell.X + .0f) * _tileSizeX, 0, (cell.Z + .0f) * _tileSizeZ);
    }
}
