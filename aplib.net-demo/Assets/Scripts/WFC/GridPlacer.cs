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
        /// Represents the key object.
        /// </summary>
        [SerializeField]
        private GameObject _keyPrefab;

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

            Cell randomPlayerSpawn = _grid.GetRandomFilledCell();

            SetRandomPLayerSpawn(randomPlayerSpawn);

            // JoinConnectedComponentsWithTeleporters();

            PlaceDoorsBetweenConnectedComponents(randomPlayerSpawn);
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
        private void PlaceDoorInDirection(int x, int z, Room room, Direction direction, List<Cell> cells)
        {
            Vector3 roomPosition = new(x * _tileSizeX, 0, z * _tileSizeZ);

            // Get (half of) the depth of the door model
            float doorDepthExtend = _doorRenderer.bounds.extents.z;

            // Calculate the distance from the room center to where a door should be placed
            float doorDistanceFromRoomCenter = _tileSizeX / 2f - doorDepthExtend;

            Quaternion roomRotation = Quaternion.Euler(0, room.Facing.RotationDegrees(), 0);

            // Calculate where the door should be placed
            Vector3 relativeDoorPosition = direction switch
            {
                North => new Vector3(0, 0, doorDistanceFromRoomCenter),
                East => new Vector3(doorDistanceFromRoomCenter, 0, 0),
                South => new Vector3(0, 0, -doorDistanceFromRoomCenter),
                West => new Vector3(-doorDistanceFromRoomCenter, 0, 0),
                _ => throw new UnityException("Invalid direction when placing door")
            };
            Vector3 doorPosition = roomPosition + relativeDoorPosition;

            // Calculate the rotation the door should have
            Quaternion relativeDoorRotation = Quaternion.Euler(0, direction.RotationDegrees(), 0);
            Quaternion doorRotation = roomRotation * relativeDoorRotation;

            // Spawn the door and key
            GameObject instantiatedDoorPrefab = Instantiate(_doorPrefab, doorPosition, doorRotation, transform);
            Door doorComponent = instantiatedDoorPrefab.GetComponentInChildren<Door>();

            Cell itemCell = cells[_random.Next(cells.Count)];

            itemCell.ContainsItem = true;

            GameObject instantiatedKeyPrefab = Instantiate(_keyPrefab, CentreOfCell(itemCell) + Vector3.up, doorRotation, transform);
            Key keyComponent = instantiatedKeyPrefab.GetComponentInChildren<Key>();
            keyComponent.Id = doorComponent.DoorId;
        }

        /// <summary>
        /// Sets the player spawn point to a random room.
        /// </summary>
        /// <param name="playerSpawnCell">The cell where the player should spawn.</param>
        private void SetRandomPLayerSpawn(Cell playerSpawnCell)
        {
            GameObject player = GameObject.FindWithTag("Player");

            if (player is null)
            {
                Debug.LogWarning("Player not found!");
                return;
            }

            Vector3 playerHeightOffset = Vector3.up * 0.7f; // Distance from the floor

            Vector3 spawningPoint = CentreOfCell(playerSpawnCell) + playerHeightOffset;

            Rigidbody playerRigidbody = player.GetComponent<Rigidbody>();
            playerRigidbody.position = spawningPoint;

            GameObject respawnPoint = GameObject.FindWithTag("Respawn");

            respawnPoint.transform.position = spawningPoint;
        }

        /// <summary>
        /// Returns the center of the cell in world coordinates.
        /// </summary>
        /// <param name="cell">The cell for which the center needs to be calculated.</param>
        /// <returns>The center of the cell in world coordinates.</returns>
        private Vector3 CentreOfCell(Cell cell) => new(cell.X * _tileSizeX, 0, cell.Z * _tileSizeZ);

        private (ISet<Cell> connectedComponent, ISet<Cell> neighbouringRooms) FindCellComponent(Cell cell,
            List<(ISet<Cell> connectedComponent, ISet<Cell> neighbouringRooms)> connectedComponents) => connectedComponents.Find(cc => cc.connectedComponent.Any(c => cell == c));

        private void PlaceDoorsBetweenConnectedComponents(Cell startCell)
        {
            List<(ISet<Cell> connectedComponent, ISet<Cell> neighbouringRooms)> connectedComponents = _grid.DetermineConnectedComponentsBetweenDoors();

            (ISet<Cell> startComponent, ISet<Cell> neighbouringRooms) = FindCellComponent(startCell, connectedComponents);

            connectedComponents.Remove((startComponent, neighbouringRooms));

            ColorConnectedComponent(startComponent);

            while (neighbouringRooms.Count > 0)
            {
                Cell neighbouringCell = neighbouringRooms.First();

                neighbouringRooms.Remove(neighbouringCell);

                IEnumerable<Cell> neighbouringCells = _grid.Get4NeighbouringCells(neighbouringCell);

                foreach (Cell cell in neighbouringCells)
                {
                    if (!startComponent.Contains(cell) || neighbouringCell.Tile is not Room && cell.Tile is not Room) continue;

                    Direction? direction = _grid.GetDirection(neighbouringCell, cell);
                    if (direction is null) continue;

                    if (neighbouringCell.Tile is not Room room)
                        PlaceDoorInDirection(cell.X, cell.Z, (Room)cell.Tile, direction.Value.Opposite(), startComponent.Where(c => !c.ContainsItem).ToList());
                    else
                        PlaceDoorInDirection(neighbouringCell.X, neighbouringCell.Z, room, direction.Value, startComponent.Where(c => !c.ContainsItem).ToList());

                    (ISet<Cell> usedComponent, ISet<Cell> usedNeighbouringRooms) = FindCellComponent(cell, connectedComponents);

                    if (usedComponent is null || usedNeighbouringRooms is null) continue;

                    connectedComponents.Remove((usedComponent, usedNeighbouringRooms));

                    ColorConnectedComponent(usedComponent);

                    usedNeighbouringRooms.Remove(cell);

                    startComponent.UnionWith(usedComponent);
                    neighbouringRooms.UnionWith(usedNeighbouringRooms);
                }

                (ISet<Cell> neighbouringCellComponent, ISet<Cell> neighbouringCellRooms) = FindCellComponent(neighbouringCell, connectedComponents);
                connectedComponents.Remove((neighbouringCellComponent, neighbouringCellRooms));

                if (neighbouringCellComponent is null) continue;

                ColorConnectedComponent(neighbouringCellComponent);

                startComponent.UnionWith(neighbouringCellComponent);
                neighbouringRooms.UnionWith(neighbouringCellRooms.Except(startComponent));
            }
        }

        private void ColorConnectedComponent(ISet<Cell> connectedComponent)
        {
            Color color = GetUnusedColor();
            foreach (Cell cell in connectedComponent)
                cell.Tile.GameObject.GetComponent<MeshRenderer>().material.color = color;
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
            Color.red,
            Color.blue,
            Color.green,
            Color.yellow,
            Color.magenta,
            Color.cyan,
            Color.black,
            Color.gray,
            Color.Lerp(Color.red, Color.blue, 0.5f),
            Color.Lerp(Color.red, Color.yellow, 0.5f),
            Color.Lerp(Color.yellow, Color.blue, 0.5f),
            Color.Lerp(Color.black, Color.red, 0.5f),
            Color.Lerp(Color.white, Color.red, 0.5f),
            Color.Lerp(Color.black, Color.blue, 0.5f),
            Color.Lerp(Color.white, Color.blue, 0.5f),
            Color.Lerp(Color.black, Color.green, 0.5f),
            Color.Lerp(Color.white, Color.green, 0.5f),
            Color.Lerp(Color.black, Color.yellow, 0.5f),
            Color.Lerp(Color.white, Color.yellow, 0.5f),
            Color.Lerp(Color.black, Color.magenta, 0.5f),
            Color.Lerp(Color.white, Color.magenta, 0.5f),
            Color.Lerp(Color.black, Color.cyan, 0.5f),
            Color.Lerp(Color.white, Color.cyan, 0.5f),
            Color.Lerp(Color.red, Color.blue, 0.25f),
            Color.Lerp(Color.red, Color.yellow, 0.25f),
            Color.Lerp(Color.yellow, Color.blue, 0.25f),
            Color.Lerp(Color.black, Color.red, 0.25f),
            Color.Lerp(Color.white, Color.red, 0.25f),
            Color.Lerp(Color.black, Color.blue, 0.25f),
            Color.Lerp(Color.white, Color.blue, 0.25f),
            Color.Lerp(Color.black, Color.green, 0.25f),
            Color.Lerp(Color.white, Color.green, 0.25f),
            Color.Lerp(Color.black, Color.yellow, 0.25f),
            Color.Lerp(Color.white, Color.yellow, 0.25f),
            Color.Lerp(Color.black, Color.magenta, 0.25f),
            Color.Lerp(Color.white, Color.magenta, 0.25f),
            Color.Lerp(Color.black, Color.cyan, 0.25f),
            Color.Lerp(Color.white, Color.cyan, 0.25f),
        };

        private static int _colorIndex = -1;
        private static Color GetUnusedColor() => _colors[_colorIndex = (_colorIndex + 1) % _colors.Length];
    }
}
