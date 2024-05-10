using Assets.Scripts.Doors;
using Assets.Scripts.Tiles;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;
using static Assets.Scripts.Tiles.Direction;
using ConnectedComponent = System.Collections.Generic.ISet<Assets.Scripts.Wfc.Cell>;
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
        public Grid Grid { get; private set; }

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
        /// The teleporter prefab, used to link connected components.
        /// </summary>
        [SerializeField]
        private GameObject _teleporterPrefab;

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
        /// The height of the offset of where we place the teleporter, with respect to the cell's floor.
        /// </summary>
        private readonly Vector3 _teleporterHeightOffset = Vector3.up * 0.7f;

        /// <summary>
        /// The distance from the floor to the player localpos.
        /// </summary>
        private readonly Vector3 _playerHeightOffset = Vector3.up * 0.7f;


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

            Cell randomPlayerSpawn = Grid.GetRandomFilledCell();

            SetRandomPLayerSpawn(randomPlayerSpawn);

            JoinConnectedComponentsWithTeleporters();

            PlaceDoorsBetweenConnectedComponents(randomPlayerSpawn);
        }

        /// <summary>
        /// Makes the grid.
        /// </summary>
        private void MakeGrid()
        {
            Grid = new Grid(_gridWidthX, _gridWidthZ, _random);

            Grid.Init();

            int numberOfRooms = 0;

            while (numberOfRooms < _amountOfRooms)
            {
                Grid.PlaceRandomRoom();
                numberOfRooms++;
            }

            while (!Grid.IsFullyCollapsed())
            {
                List<Cell> lowestEntropyCells = Grid.GetLowestEntropyCells();

                int index = _random.Next(lowestEntropyCells.Count);

                Cell cell = lowestEntropyCells[index];
                cell.Tile = cell.Candidates[_random.Next(cell.Candidates.Count)];
                cell.Candidates.Clear();

                Grid.RemoveUnconnectedNeighbourCandidates(cell);
            }
        }

        /// <summary>
        /// Places the grid in the world.
        /// </summary>
        private void PlaceGrid()
        {
            GameObject tiles = new("Tiles") { transform = { parent = transform } };

            for (int z = 0; z < Grid.Height; z++)
            {
                for (int x = 0; x < Grid.Width; x++) PlaceTile(x, z, Grid[x, z].Tile, tiles.transform);
            }
        }

        /// <summary>
        /// Places a tile at the specified coordinates in the world.
        /// </summary>
        /// <param name="x">The x-coordinates of the room.</param>
        /// <param name="z">The z-coordinates of the room.</param>
        /// <param name="tile">The tile that needs to be placed.</param>
        /// <param name="parent">The parent of the teleporter.</param>
        private void PlaceTile(int x, int z, Tile tile, Transform parent)
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
                parent
            );
        }

        /// <summary>
        /// Calculates the centre of a cell's floor in world coordinates.
        /// </summary>
        /// <param name="cell">The cell to calculate its centre of.</param>
        /// <returns>The real-world coordinates of the centre of the cell's floor.</returns>
        public Vector3 CentreOfCell(Cell cell) => new(cell.X * _tileSizeX, 0, cell.Z * _tileSizeZ);

        /// <summary>
        /// Place the doors for the given room in the world. Which doors need to be spawned is determined from the
        /// allowed directions of the room.
        /// </summary>
        /// <param name="x">The x-position of the room, in the grid.</param>
        /// <param name="z">The z-position of the room, in the grid.</param>
        /// <param name="room">The room for which the doors need to be spawned.</param>
        /// <param name="parent">The parent of the teleporter.</param>
        // ReSharper disable once SuggestBaseTypeForParameter
        private void PlaceDoorInDirection(int x, int z, Room room, Direction direction, List<Cell> cells, Transform parent)
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
            GameObject instantiatedDoorPrefab = Instantiate(_doorPrefab, doorPosition, doorRotation, parent);
            Door doorComponent = instantiatedDoorPrefab.GetComponentInChildren<Door>();

            Cell itemCell = cells[_random.Next(cells.Count)];

            itemCell.ContainsItem = true;

            GameObject instantiatedKeyPrefab = Instantiate(_keyPrefab, CentreOfCell(itemCell) + Vector3.up, doorRotation, parent);
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

            if (player is null) throw new UnityException("No player was found.");

            Vector3 spawningPoint = CentreOfCell(playerSpawnCell) + _playerHeightOffset;

            Rigidbody playerRigidbody = player.GetComponent<Rigidbody>();
            playerRigidbody.position = spawningPoint;

            GameObject respawnPoint = GameObject.FindWithTag("Respawn");

            respawnPoint.transform.position = spawningPoint;
        }

        private (ISet<Cell> connectedComponent, ISet<Cell> neighbouringRooms) FindCellComponent(Cell cell,
            List<(ISet<Cell> connectedComponent, ISet<Cell> neighbouringRooms)> connectedComponents) => connectedComponents.Find(cc => cc.connectedComponent.Any(c => cell == c));

        private void PlaceDoorsBetweenConnectedComponents(Cell startCell)
        {
            GameObject doors = new("Doors and keys") { transform = { parent = transform } };

            List<(ISet<Cell> connectedComponent, ISet<Cell> neighbouringRooms)> connectedComponents = Grid.DetermineConnectedComponentsBetweenDoors();

            (ISet<Cell> startComponent, ISet<Cell> neighbouringRooms) = FindCellComponent(startCell, connectedComponents);

            connectedComponents.Remove((startComponent, neighbouringRooms));

            ColorConnectedComponent(startComponent);

            while (neighbouringRooms.Count > 0)
            {
                Cell neighbouringCell = neighbouringRooms.First();

                neighbouringRooms.Remove(neighbouringCell);

                IEnumerable<Cell> neighbouringCells = Grid.Get4NeighbouringCells(neighbouringCell);

                foreach (Cell cell in neighbouringCells)
                {
                    if (!startComponent.Contains(cell) || neighbouringCell.Tile is not Room && cell.Tile is not Room) continue;

                    Direction? direction = Grid.GetDirection(neighbouringCell, cell);
                    if (direction is null) continue;

                    if (neighbouringCell.Tile is not Room room)
                        PlaceDoorInDirection(cell.X, cell.Z, (Room)cell.Tile, direction.Value.Opposite(), startComponent.Where(c => !c.ContainsItem).ToList(), doors.transform);
                    else
                        PlaceDoorInDirection(neighbouringCell.X, neighbouringCell.Z, room, direction.Value, startComponent.Where(c => !c.ContainsItem).ToList(), doors.transform);

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
        /// Linearly connect all connected components, such that every connected component has two bidirectional teleporters.
        /// The last teleporter is connected to itself, to indicate that it should not teleport the player anywhere.
        /// </summary>
        /// <remarks>Assumes that every connected component has at least two cells.</remarks>
        private void JoinConnectedComponentsWithTeleporters()
        {
            GameObject teleporters = new("Teleporters") { transform = { parent = transform } };

            // Prepare some variables
            using IEnumerator<ConnectedComponent> connectedComponentsEnumerator = Grid.DetermineConnectedComponents().GetEnumerator();

            Teleporter.Teleporter previousExitTeleporter = null;
            int connectedComponentsProcessed = 0;

            // Keep connecting the next connected components with the previous one, by placing a pair of teleporters.
            while (connectedComponentsEnumerator.MoveNext())
            {
                // Link current and previous connected component bidirectionally,
                // if the current connected component is not the first connected component.
                if (connectedComponentsProcessed > 0)
                {
                    // Place an entry teleporter, so that the previous connected component can be linked through this teleporter.
                    Cell nextEntryCell = connectedComponentsEnumerator.Current!.First();
                    Teleporter.Teleporter entryTeleporter = PlaceTeleporter(CentreOfCell(nextEntryCell) + _teleporterHeightOffset, teleporters.transform);

                    // Link the entry teleporter back to the previous connected component, bidirectionally.
                    previousExitTeleporter!.TargetTeleporter = entryTeleporter;
                    entryTeleporter.TargetTeleporter = previousExitTeleporter;

                    // Create an OffMeshLink between the two teleporters.
                    OffMeshLink offMeshLink = previousExitTeleporter.gameObject.AddComponent<OffMeshLink>();
                    offMeshLink.startTransform = previousExitTeleporter.transform;
                    offMeshLink.endTransform = entryTeleporter.transform;
                    offMeshLink.autoUpdatePositions = true;
                    offMeshLink.area = NavMesh.GetAreaFromName("PlayerWalkable");
                }

                // Place the exit teleporter of the current connected component at the end of the connected component.
                // (Assuming that the connected component has at least two cells, `nextExitCell` will never equal `nextEntryCell`)
                Cell nextExitCell = connectedComponentsEnumerator.Current!.Last();
                Teleporter.Teleporter exitTeleporter = PlaceTeleporter(CentreOfCell(nextExitCell) + _teleporterHeightOffset, teleporters.transform);

                // Update iteration progress
                previousExitTeleporter = exitTeleporter;
                connectedComponentsProcessed++;
            }

            // If at least one exit portal has been placed, the final exit teleporter will not be linked to
            // another connected component, for this final exit teleporter belongs to the final connected component.
            if (connectedComponentsProcessed > 0) Destroy(previousExitTeleporter!.gameObject);
        }

        /// <summary>
        /// Places a teleporter at a given location.
        /// </summary>
        /// <param name="coordinates">The coordinates of the teleporter.</param>
        /// <param name="parent">The parent of the teleporter.</param>
        /// <returns>A reference to the <see cref="Teleporter"/> component of the teleporter.</returns>
        /// <remarks>All teleporters are clustered under a 'Teleporter' empty gameobject.</remarks>
        private Teleporter.Teleporter PlaceTeleporter(Vector3 coordinates, Transform parent) => Instantiate(_teleporterPrefab, coordinates, Quaternion.identity, parent).GetComponent<Teleporter.Teleporter>();

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
