using Assets.Scripts.Tiles;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;
using ConnectedComponent = System.Collections.Generic.ISet<Assets.Scripts.Wfc.Cell>;
using Random = System.Random;

namespace Assets.Scripts.Wfc
{
    /// <summary>
    /// Represents the grid placer. This class is responsible for placing the grid in the world. It also places the keys
    /// and doors between the rooms, and the teleporters between the connected components.
    /// </summary>
    [RequireComponent(typeof(GameObjectPlacer))]
    public class GridPlacer : MonoBehaviour
    {
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
        /// The height of the offset of where we place the teleporter, with respect to the cell's floor.
        /// </summary>
        private readonly Vector3 _teleporterHeightOffset = Vector3.up * .7f;

        /// <summary>
        /// Represents the game object placer.
        /// </summary>
        private GameObjectPlacer _gameObjectPlacer;

        /// <summary>
        /// The distance from the floor to the player localpos.
        /// </summary>
        private Vector3 _playerHeightOffset;

        /// <summary>
        /// The height of the offset of where we place the teleporter, with respect to the cell's floor.
        /// </summary>
        private Random _random = new();

        /// <summary>
        /// Represents the grid.
        /// </summary>
        public Grid Grid { get; private set; }

        /// <summary>
        /// This contains the whole 'pipeline' of level generation, including initialising the grid and placing teleporters.
        /// </summary>
        public void Awake()
        {
            _gameObjectPlacer = GetComponent<GameObjectPlacer>();
            _gameObjectPlacer.Initialize();

            GameObject player = GameObject.FindWithTag("Player");

            if (player == null) throw new UnityException("No player was found.");

            float playerHeight = player.GetComponent<CapsuleCollider>().height;
            Vector3 playerHeightOffset = new(0, playerHeight, 0);

            _playerHeightOffset = playerHeightOffset;

            MakeScene();
        }

        /// <summary>
        /// Waits for the specified time and then makes the scene.
        /// </summary>
        /// <param name="waitTime">The time to wait before making the scene.</param>
        public void WaitBeforeMakeScene(float waitTime = 0.01f) => StartCoroutine(WaitThenMakeScene(waitTime));

        /// <summary>
        /// Waits for a certain amount of time.
        /// </summary>
        /// <param name="waitTime">The time to wait before making the scene.</param>
        /// <returns>An <see cref="IEnumerator" /> that can be used to wait for a certain amount of time.</returns>
        private IEnumerator WaitThenMakeScene(float waitTime)
        {
            yield return new WaitForSeconds(waitTime);

            MakeScene();
        }

        /// <summary>
        /// Makes the scene.
        /// </summary>
        /// <exception cref="Exception">The amount of rooms is larger than the available places in the grid.</exception>
        private void MakeScene()
        {
            if (_amountOfRooms > _gridWidthX * _gridWidthZ)
                throw new Exception("The amount of rooms is larger than the available places in the grid.");

            if (_useSeed) _random = new Random(_seed);

            MakeGrid();

            Cell randomPlayerSpawn = Grid.GetRandomFilledCell();

            SetPlayerSpawn(randomPlayerSpawn);

            SwitchTile(randomPlayerSpawn);

            PlaceGrid();

            JoinConnectedComponentsWithTeleporters();

            PlaceDoorsBetweenConnectedComponents(randomPlayerSpawn);

            SpawnItems();
        }

        public void SwitchTile(Cell startTile)
        {
            switch (startTile.Tile)
            {
                case Corner:
                    startTile.Tile = new StartCorner();
                    break;
                case Crossing:
                    startTile.Tile = new StartCrossing();
                    break;
                case DeadEnd:
                    startTile.Tile = new StartDeadEnd();
                    break;
                case Room:
                    startTile.Tile = new StartRoom();
                    break;
                case Straight:
                    startTile.Tile = new StartStraight();
                    break;
                case TSection:
                    startTile.Tile = new StartTSection();
                    break;
                default:
                    break;
            }
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
            GameObject tiles = CreateGameObject("Tiles", transform);

            for (int z = 0; z < Grid.Height; z++)
            {
                for (int x = 0; x < Grid.Width; x++)
                    _gameObjectPlacer.PlaceTile(x, z, Grid[x, z].Tile, tiles.transform);
            }
        }

        /// <summary>
        /// Sets the player spawn point to a random room.
        /// </summary>
        /// <param name="playerSpawnCell">The cell where the player should spawn.</param>
        private void SetPlayerSpawn(Cell playerSpawnCell)
        {
            GameObject player = GameObject.FindWithTag("Player");

            if (player == null) throw new UnityException("No player was found.");

            Vector3 spawningPoint = _gameObjectPlacer.CenterOfCell(playerSpawnCell) + _playerHeightOffset;

            Rigidbody playerRigidbody = player.GetComponent<Rigidbody>();
            playerRigidbody.position = spawningPoint;

            GameObject respawnPoint = GameObject.FindWithTag("Respawn");

            if (respawnPoint == null) return;

            respawnPoint.transform.position = spawningPoint;
            Area area = respawnPoint.GetComponent<Area>();
            area.Bounds = new Bounds(spawningPoint, area.Bounds.extents);
        }

        /// <summary>
        /// Finds the connected component that contains the given cell.
        /// </summary>
        /// <param name="cell">The cell to find the connected component for.</param>
        /// <param name="connectedComponents">The list of connected components to search through.</param>
        /// <returns>The connected component that contains the given cell.</returns>
        private static (ConnectedComponent connectedComponent, ConnectedComponent neighbouringRooms)
            FindCellConnectedComponent(
                Cell cell,
                List<(ConnectedComponent connectedComponent, ConnectedComponent neighbouringRooms)>
                    connectedComponents) =>
            connectedComponents.Find(cc => cc.connectedComponent.Contains(cell));

        /// <summary>
        /// Finds and removes a connected component from the list of connected components.
        /// </summary>
        /// <param name="cell">The cell for which the connected component is to be found.</param>
        /// <param name="connectedComponents">The list of connected components to search through.</param>
        /// <returns>A tuple containing the connected component and its neighbouring rooms that contains the given cell.</returns>
        private static (ConnectedComponent connectedComponent, ConnectedComponent neighbouringRooms)
            FindAndRemoveCellConnectedComponent(Cell cell,
                List<(ConnectedComponent connectedComponent, ConnectedComponent neighbouringRooms)> connectedComponents)
        {
            (ConnectedComponent connectedComponent, ConnectedComponent neighbouringRooms) =
                FindCellConnectedComponent(cell, connectedComponents);

            connectedComponents.Remove((connectedComponent, neighbouringRooms));

            return (connectedComponent, neighbouringRooms);
        }


        /// <summary>
        /// Gets the cell at a given position.
        /// </summary>
        /// <param name="position">The position to get the cell for.</param>
        /// <returns>The cell at the given position.</returns>
        private Cell GetCellAtPosition(Vector3 position)
        {
            (int x, int z) = _gameObjectPlacer.GetCellCoordinates(position);
            return Grid[x, z];
        }

        /// <summary>
        /// Places doors between connected components. Connected components are components of cells that are connected and
        /// disconnected by rooms.
        /// </summary>
        /// <param name="startCell">The cell to start the search from.</param>
        /// <exception cref="UnityException">No teleporters were found.</exception>
        private void PlaceDoorsBetweenConnectedComponents(Cell startCell)
        {
            GameObject doors = CreateGameObject("Doors and keys", transform);
            List<(ConnectedComponent connectedComponent, ConnectedComponent neighbouringRooms)> connectedComponents =
                Grid.DetermineConnectedComponentsBetweenDoors();

            GameObject teleporters = GameObject.Find("Teleporters");
            if (teleporters is null) throw new UnityException("No teleporters were found.");

            List<Teleporter.Teleporter> teleporterList = GetAllEntryTeleporters(teleporters);

            MergeConnectedComponentsJoinedByTeleporterPair(teleporterList, connectedComponents);

            (ConnectedComponent startComponent, ConnectedComponent neighbouringRooms) =
                FindAndRemoveCellConnectedComponent(startCell, connectedComponents);

            ProcessNeighbouringRooms(startComponent, neighbouringRooms, connectedComponents, doors);
        }

        /// <summary>
        /// Creates a new game object with a given name and parent.
        /// </summary>
        /// <param name="objectName">The name of the game object.</param>
        /// <param name="parent">The parent of the game object.</param>
        /// <returns>The newly created game object.</returns>
        private static GameObject CreateGameObject(string objectName, Transform parent) =>
            new(objectName) { transform = { parent = parent } };

        /// <summary>
        /// Gets the unique teleporters from a given game object.
        /// </summary>
        /// <param name="teleporters">The game object to get the teleporters from.</param>
        /// <returns>A list of unique teleporters.</returns>
        private static List<Teleporter.Teleporter> GetAllEntryTeleporters(GameObject teleporters)
        {
            List<Teleporter.Teleporter> teleporterList = new();
            foreach (Transform child in teleporters.transform)
            {
                Teleporter.Teleporter teleporter = child.gameObject.GetComponent<Teleporter.Teleporter>();
                if (teleporterList.Contains(teleporter.TargetTeleporter)) continue;
                teleporterList.Add(teleporter);
            }

            return teleporterList;
        }

        /// <summary>
        /// Merges teleporter connected components.
        /// </summary>
        /// <param name="teleporterList">The list of teleporters to merge.</param>
        /// <param name="connectedComponents">The list of connected components to merge.</param>
        private void MergeConnectedComponentsJoinedByTeleporterPair(List<Teleporter.Teleporter> teleporterList,
            List<(ConnectedComponent connectedComponent, ConnectedComponent neighbouringRooms)> connectedComponents)
        {
            foreach (Teleporter.Teleporter teleporter in teleporterList)
            {
                if (teleporter.TargetTeleporter == null) continue;

                Cell teleporterCell = GetCellAtPosition(teleporter.transform.position);
                (ConnectedComponent component, ConnectedComponent ns) =
                    FindCellConnectedComponent(teleporterCell, connectedComponents);

                Cell targetCell = GetCellAtPosition(teleporter.TargetTeleporter.transform.position);
                (ConnectedComponent targetComponent, ConnectedComponent targetNs) =
                    FindAndRemoveCellConnectedComponent(targetCell, connectedComponents);

                component.UnionWith(targetComponent);
                ns.UnionWith(targetNs);
            }
        }

        /// <summary>
        /// Gets the available cells in a given component.
        /// </summary>
        /// <param name="component">The component to get the available cells for.</param>
        /// <returns>A list of available cells.</returns>
        private static List<Cell> GetEmptyCells(ConnectedComponent component) =>
            component.Where(c => !c.CannotAddItem).ToList();

        /// <summary>
        /// Places a door between two cells in a given direction.
        /// </summary>
        /// <param name="cell1">The first cell to place the door in.</param>
        /// <param name="cell2">The second cell to place the door in.</param>
        /// <param name="direction">The direction in which the door should be placed.</param>
        /// <param name="startComponent">The component to start the search from.</param>
        /// <param name="parent">The parent of the door.</param>
        private void PlaceDoor(Cell cell1, Cell cell2, Direction direction, ConnectedComponent startComponent,
            Transform parent)
        {
            List<Cell> emptyCells = GetEmptyCells(startComponent);
            Cell itemCell = emptyCells[_random.Next(emptyCells.Count)];

            if (cell1.Tile is Room room)
                _gameObjectPlacer.PlaceDoorInDirection(cell1.X, cell1.Z, room, direction, itemCell, parent);
            else if (cell2.Tile is Room room2)
                _gameObjectPlacer.PlaceDoorInDirection(cell2.X, cell2.Z, room2, direction.Opposite(), itemCell, parent);
        }

        /// <summary>
        /// Processes the neighbouring rooms of a given component. It will place doors between the rooms and connect the
        /// components.
        /// </summary>
        /// <param name="startComponent">The component to start the search from.</param>
        /// <param name="neighbouringRooms">The neighbouring rooms to process.</param>
        /// <param name="connectedComponents">The list of connected components to process.</param>
        /// <param name="doors">The parent of the doors.</param>
        private void ProcessNeighbouringRooms(ConnectedComponent startComponent, ConnectedComponent neighbouringRooms,
            List<(ConnectedComponent connectedComponent, ConnectedComponent neighbouringRooms)> connectedComponents,
            GameObject doors)
        {
            ConnectedComponent lastComponent = startComponent;

            while (neighbouringRooms.Count > 0)
            {
                Cell neighbouringCell = neighbouringRooms.First();
                neighbouringRooms.Remove(neighbouringCell);

                foreach (Cell cell in Grid.Get4NeighbouringCells(neighbouringCell))
                {
                    if (!startComponent.Contains(cell)) continue;

                    Direction? direction = Grid.GetDirection(neighbouringCell, cell);
                    if (direction == null) continue;

                    PlaceDoor(neighbouringCell, cell, direction.Value, startComponent, doors.transform);

                    (ConnectedComponent usedComponent, ConnectedComponent usedNeighbouringRooms) =
                        FindAndRemoveCellConnectedComponent(cell, connectedComponents);

                    if (usedComponent == null) continue;

                    startComponent.UnionWith(usedComponent);
                    neighbouringRooms.UnionWith(usedNeighbouringRooms);
                }

                (ConnectedComponent neighbouringCellComponent, ConnectedComponent neighbouringCellRooms) =
                    FindAndRemoveCellConnectedComponent(neighbouringCell, connectedComponents);

                if (neighbouringCellComponent == null) continue;

                lastComponent = neighbouringCellComponent;

                startComponent.UnionWith(neighbouringCellComponent);
                neighbouringRooms.UnionWith(neighbouringCellRooms.Except(startComponent));
            }

            List<Cell> lastComponentCells = GetEmptyCells(lastComponent);

            if (lastComponentCells.Count == 0) lastComponentCells = lastComponent.ToList();

            Cell randomCell = lastComponentCells[_random.Next(lastComponentCells.Count)];
            _gameObjectPlacer.PlaceEndItem(randomCell, transform);
        }

        /// <summary>
        /// Linearly connect all connected components, such that every connected component has two bidirectional teleporters.
        /// The last teleporter is connected to itself, to indicate that it should not teleport the player anywhere.
        /// </summary>
        /// <remarks>Assumes that every connected component has at least two cells.</remarks>
        private void JoinConnectedComponentsWithTeleporters()
        {
            GameObject teleporters = CreateGameObject("Teleporters", transform);

            // Prepare some variables
            using IEnumerator<ConnectedComponent> connectedComponentsEnumerator =
                Grid.DetermineConnectedComponents().GetEnumerator();

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
                    nextEntryCell.CannotAddItem = true;
                    Teleporter.Teleporter entryTeleporter =
                        _gameObjectPlacer.PlaceTeleporter(
                            _gameObjectPlacer.CenterOfCell(nextEntryCell) + _teleporterHeightOffset,
                            teleporters.transform);

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
                nextExitCell.CannotAddItem = true; // No item can be placed in this cell anymore.
                Teleporter.Teleporter exitTeleporter =
                    _gameObjectPlacer.PlaceTeleporter(
                        _gameObjectPlacer.CenterOfCell(nextExitCell) + _teleporterHeightOffset,
                        teleporters.transform);

                // Update iteration progress
                previousExitTeleporter = exitTeleporter;
                connectedComponentsProcessed++;
            }

            // If at least one exit portal has been placed, the final exit teleporter will not be linked to
            // another connected component, for this final exit teleporter belongs to the final connected component.
            if (connectedComponentsProcessed > 0) Destroy(previousExitTeleporter!.gameObject);
        }

        /// <summary>
        /// Spawns the items in the grid.
        /// </summary>
        private void SpawnItems()
        {
            List<Cell> cells = Grid.GetAllCellsNotContainingItems();

            _gameObjectPlacer.SpawnItems(cells, _random);
        }
    }
}
