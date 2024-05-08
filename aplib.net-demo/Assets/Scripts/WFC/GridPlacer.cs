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

            JoinConnectedComponentsWithTeleporters();

            PlacePlayer();
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
            for (int z = 0; z < Grid.Height; z++)
            {
                for (int x = 0; x < Grid.Width; x++) PlaceTile(x, z, Grid[x, z].Tile);
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
        /// Linearly connect all connected components, such that every connected component has two bidirectional teleporters.
        /// The last teleporter is connected to itself, to indicate that it should not teleport the player anywhere.
        /// </summary>
        /// <remarks>Assumes that every connected component has at least two cells.</remarks>
        private void JoinConnectedComponentsWithTeleporters()
        {
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
                    Teleporter.Teleporter entryTeleporter = PlaceTeleporter(CentreOfCell(nextEntryCell) + _teleporterHeightOffset);

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
                Teleporter.Teleporter exitTeleporter = PlaceTeleporter(CentreOfCell(nextExitCell) + _teleporterHeightOffset);

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
        /// <returns>A reference to the <see cref="Teleporter"/> component of the teleporter.</returns>
        /// <remarks>All teleporters are clustered under a 'Teleporter' empty gameobject.</remarks>
        private Teleporter.Teleporter PlaceTeleporter(Vector3 coordinates)
        {
            // Give all teleporters a parent object for organization.
            GameObject teleportersParent = GameObject.Find("Teleporters");
            if (teleportersParent is null)
            {
                teleportersParent = new GameObject("Teleporters");
            }

            return Instantiate(_teleporterPrefab, coordinates, Quaternion.identity, teleportersParent.transform)
                .GetComponent<Teleporter.Teleporter>();
        }

        /// <summary>
        /// Calculates the centre of a cell's floor in world coordinates.
        /// </summary>
        /// <param name="cell">The cell to calculate its centre of.</param>
        /// <returns>The real-world coordinates of the centre of the cell's floor.</returns>
        public Vector3 CentreOfCell(Cell cell) => new(cell.X * _tileSizeX, 0, cell.Z * _tileSizeZ);

        /// <summary>
        /// Finds the first room in the grid, and places the player in the centre of that room.
        /// </summary>
        /// <exception cref="UnityException">If no player is found, this step fails.</exception>
        /// <remarks>Assumes that there is at least one room.</remarks>
        private void PlacePlayer()
        {
            GameObject player = GameObject.FindWithTag("Player");
            if (player == null) throw new UnityException("No player was found.");
            Rigidbody rigidBody = player.GetComponent<Rigidbody>();

            // Find the first room that is not empty and place the player there.
            for (int i = 0; i < Grid.NumberOfCells; i++)
            {
                if (Grid[i].Tile is not Room) continue;

                // Room found, set player position and break.
                rigidBody.position = CentreOfCell(Grid[i]) + _playerHeightOffset;
                break;
            }
        }
    }
}
