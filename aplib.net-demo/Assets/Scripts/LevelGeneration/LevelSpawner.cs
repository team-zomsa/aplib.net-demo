using Assets.Scripts.Tiles;
using System;
using System.Collections.Generic;
using System.Linq;
using ThreadSafeRandom;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;
using WFC;
using ConnectedComponent = System.Collections.Generic.ISet<WFC.Cell>;
using Grid = WFC.Grid;
using Random = UnityEngine.Random;

namespace LevelGeneration
{
    /// <summary>
    /// Represents the grid placer. This class is responsible for placing the grid in the world. It also places the keys
    /// and doors between the rooms, and the teleporters between the connected components.
    /// </summary>
    [RequireComponent(typeof(GameObjectPlacer))]
    [RequireComponent(typeof(SpawningExtensions))]
    public class LevelSpawner : MonoBehaviour
    {
        private readonly float _maxSaturation = 1f;

        private readonly float _maxValue = 1f;

        private readonly float _minSaturation = 0.5f;

        private readonly float _minValue = 0.3f;

        /// <summary>
        /// The height of the offset of where we place the teleporter, with respect to the cell's floor.
        /// </summary>
        private readonly Vector3 _teleporterHeightOffset = Vector3.up * .7f;

        /// <summary>
        /// Represents the game object placer.
        /// </summary>
        private GameObjectPlacer _gameObjectPlacer;

        /// <summary>
        /// Represents the spawning extensions.
        /// </summary>
        private SpawningExtensions _spawningExtensions;

        /// <summary>
        /// Represents the grid.
        /// </summary>
        public Grid Grid { get; set; }

        public void Awake()
        {
            _gameObjectPlacer = GetComponent<GameObjectPlacer>();
            _spawningExtensions = GetComponent<SpawningExtensions>();
        }

        /// <summary>
        /// Makes the level.
        /// </summary>
        /// <exception cref="Exception">The amount of rooms is larger than the available places in the grid.</exception>
        public void MakeLevel(Cell playerSpawnCell)
        {
            UpdateTileModel(playerSpawnCell);

            PlaceGrid();

            JoinConnectedComponentsWithTeleporters();

            PlaceDoorsBetweenConnectedComponents(playerSpawnCell);

            GameObject environmentGameObject = GameObject.FindWithTag("Environment");

            if (environmentGameObject == null) return;

            NavMeshSurface navMeshSurface = environmentGameObject.GetComponent<NavMeshSurface>();
            Debug.Log("Building navmesh...");
            navMeshSurface.BuildNavMesh();
        }

        public static void UpdateTileModel(Cell startTile) => startTile.Tile.IsStart = true;

        /// <summary>
        /// Places the grid in the world.
        /// </summary>
        private void PlaceGrid()
        {
            GameObject tiles = SpawningExtensions.CreateGameObject("Tiles", transform);

            for (int z = 0; z < Grid.Height; z++)
            {
                for (int x = 0; x < Grid.Width; x++)
                {
                    _gameObjectPlacer.PlaceTile(x, z, Grid[x, z].Tile, tiles.transform);
                }
            }
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
            (int x, int z) = _spawningExtensions.GetCellCoordinates(position);
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
            GameObject doors = SpawningExtensions.CreateGameObject("Doors and keys", transform);
            List<(ConnectedComponent connectedComponent, ConnectedComponent neighbouringRooms)> connectedComponents =
                Grid.DetermineConnectedComponentsBetweenDoors();

            GameObject teleporters = GameObject.Find("Teleporters");
            if (teleporters == null) throw new UnityException("No teleporters were found.");

            List<Teleporter.Teleporter> teleporterList = GetAllEntryTeleporters(teleporters);

            MergeConnectedComponentsJoinedByTeleporterPair(teleporterList, connectedComponents);

            ColorConnectedComponent(connectedComponents.Select(t => t.connectedComponent));

            (ConnectedComponent startComponent, ConnectedComponent neighbouringRooms) =
                FindAndRemoveCellConnectedComponent(startCell, connectedComponents);

            ProcessNeighbouringRooms(startComponent, neighbouringRooms, connectedComponents, doors);
        }

        /// <summary>
        /// Colors the connected components.
        /// </summary>
        /// <param name="connectedComponents">The connected components to color.</param>
        private void ColorConnectedComponent(IEnumerable<ConnectedComponent> connectedComponents)
        {
            foreach (ConnectedComponent connectedComponent in connectedComponents)
            {
                Color componentColor = Random.ColorHSV(0f, 1f, _minSaturation, _maxSaturation, _minValue, _maxValue);
                foreach (Cell cell in connectedComponent)
                {
                    cell.Tile.GameObject.GetComponent<Renderer>().material.color = componentColor;
                }
            }
        }

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
        private (int x, int z, int x2, int z2)? PlaceDoor
        (
            Cell cell1,
            Cell cell2,
            Direction direction,
            ConnectedComponent startComponent,
            Transform parent
        )
        {
            List<Cell> emptyCells = GetEmptyCells(startComponent);

            // If there are no empty cells, we cannot place a door. This can occur when there is exactly one cell per key and placed teleporters.
            if (emptyCells.Count == 0) return null;

            Cell itemCell = emptyCells[SharedRandom.Next(emptyCells.Count)];

            if (cell1.Tile is Room room1)
            {
                _gameObjectPlacer.PlaceDoorInDirection(cell1.X, cell1.Z, room1, direction, itemCell, parent);
                return (cell1.X, cell1.Z, cell2.X, cell2.Z);
            }

            // ReSharper disable once InvertIf
            if (cell2.Tile is Room room2)
            {
                _gameObjectPlacer.PlaceDoorInDirection(cell2.X, cell2.Z, room2, direction.Opposite(), itemCell, parent);
                return (cell2.X, cell2.Z, cell1.X, cell1.Z);
            }

            return null;
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
            HashSet<(int x, int z, int x2, int z2)> placedDoors = new();

            while (neighbouringRooms.Count > 0)
            {
                Cell neighbouringCell = neighbouringRooms.First();
                neighbouringRooms.Remove(neighbouringCell);

                foreach (Cell cell in Grid.Get4NeighbouringCells(neighbouringCell))
                {
                    if (!startComponent.Contains(cell) ||
                        placedDoors.Contains((cell.X, cell.Z, neighbouringCell.X, neighbouringCell.Z)) ||
                        placedDoors.Contains((neighbouringCell.X, neighbouringCell.Z, cell.X, cell.Z)))
                        continue;

                    Direction? direction = Grid.GetDirection(neighbouringCell, cell);
                    if (direction == null) continue;

                    (int x, int z, int x2, int z2)? placed = PlaceDoor(neighbouringCell, cell, direction.Value,
                        startComponent, doors.transform);

                    if (placed == null) continue;

                    placedDoors.Add(placed.Value);

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

            Cell randomCell = lastComponentCells[SharedRandom.Next(lastComponentCells.Count)];
            _gameObjectPlacer.PlaceEndItem(randomCell, transform);
        }

        /// <summary>
        /// Linearly connect all connected components, such that every connected component has two bidirectional teleporters.
        /// The last teleporter is connected to itself, to indicate that it should not teleport the player anywhere.
        /// </summary>
        /// <remarks>Assumes that every connected component has at least two cells.</remarks>
        private void JoinConnectedComponentsWithTeleporters()
        {
            GameObject teleporters = SpawningExtensions.CreateGameObject("Teleporters", transform);

            // Prepare some variables
            using IEnumerator<ConnectedComponent> connectedComponentsEnumerator =
                Grid.DetermineConnectedComponents().GetEnumerator();

            Teleporter.Teleporter previousExitTeleporter = null;
            Cell previousExitCell = null;
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
                            _spawningExtensions.CenterOfCell(nextEntryCell) + _teleporterHeightOffset,
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
                        _spawningExtensions.CenterOfCell(nextExitCell) + _teleporterHeightOffset,
                        teleporters.transform);

                // Update iteration progress
                previousExitTeleporter = exitTeleporter;
                previousExitCell = nextExitCell;
                connectedComponentsProcessed++;
            }

            // If at least one exit portal has been placed, the final exit teleporter will not be linked to
            // another connected component, for this final exit teleporter belongs to the final connected component.
            if (connectedComponentsProcessed <= 0) return;

            Destroy(previousExitTeleporter!.gameObject);
            previousExitCell!.CannotAddItem = false;
        }
    }
}
