using Assets.Scripts.Doors;
using Assets.Scripts.Tiles;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static Assets.Scripts.Tiles.Direction;
using Random = System.Random;

namespace Assets.Scripts.Wfc
{
    [RequireComponent(typeof(GridPlacer))]
    public class GameObjectPlacer : MonoBehaviour
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
        /// Represents the spawnable items.
        /// </summary>
        [SerializeField]
        private SpawnableItems _spawnableItems;

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
        /// The teleporter prefab, used to link connected components.
        /// </summary>
        [SerializeField]
        private GameObject _teleporterPrefab;

        /// <summary>
        /// The end item prefab.
        /// </summary>
        [SerializeField]
        private GameObject _endItemPrefab;

        /// <summary>
        /// The material of the start room.
        /// </summary>
        [SerializeField]
        private Material _startRoomMat;

        /// <summary>
        /// The material of the end room.
        /// </summary>
        [SerializeField]
        private Material _endRoomMat;

        /// <summary>
        /// The depth of the door prefab.
        /// </summary>
        /// <remarks>Getting a reference to the component is expensive, so we only want to do it once.</remarks>
        private float _doorDepthExtent;

        /// <summary>
        /// The distance from the floor to the player localpos.
        /// </summary>
        private Vector3 _playerHeightOffset;

        /// <summary>
        /// Represents the spawning extensions.
        /// </summary>
        private SpawningExtensions _spawningExtensions;

        /// <summary>
        /// This contains the whole 'pipeline' of level generation, including initialising the grid and placing teleporters.
        /// </summary>
        public void Initialize()
        {
            if (!_doorPrefab.TryGetComponent(out Renderer doorRenderer))
                throw new UnityException("Door prefab does not have a renderer component.");

            _doorDepthExtent = doorRenderer.bounds.extents.z;

            GameObject player = GameObject.FindWithTag("Player");

            if (player == null) throw new UnityException("No player was found.");

            float playerHeight = player.GetComponent<CapsuleCollider>().height;
            Vector3 playerHeightOffset = new(0, playerHeight, 0);

            _playerHeightOffset = playerHeightOffset;

            if (!TryGetComponent(out _spawningExtensions))
                throw new UnityException("SpawningExtensions not found.");
        }

        /// <summary>
        /// Places a teleporter at a given location.
        /// </summary>
        /// <param name="coordinates">The coordinates of the teleporter.</param>
        /// <param name="parent">The parent of the teleporter.</param>
        /// <returns>A reference to the <see cref="Teleporter" /> component of the teleporter.</returns>
        /// <remarks>All teleporters are clustered under a 'Teleporter' empty gameobject.</remarks>
        public Teleporter.Teleporter PlaceTeleporter(Vector3 coordinates, Transform parent) =>
            Instantiate(_teleporterPrefab, coordinates, Quaternion.identity, parent)
                .GetComponent<Teleporter.Teleporter>();

        /// <summary>
        /// Spawn the end item in the given cell.
        /// </summary>
        /// <param name="cell">The cell to spawn the end item in.</param>
        /// <param name="parent">The parent of the end item.</param>
        public void PlaceEndItem(Cell cell, Transform parent)
        {
            _spawningExtensions.PlacePrefab(_endItemPrefab, cell, parent);

            // Change the cell color to end room color.
            cell.Tile.GameObject.GetComponent<Renderer>().material = _endRoomMat;
        }

        /// <summary>
        /// Spawns all items in the world.
        /// </summary>
        /// <param name="cells">The cells to spawn the items in.</param>
        /// <param name="random">The random number generator to use.</param>
        /// <exception cref="UnityException">Thrown when there are not enough empty cells to place all items.</exception>
        public void SpawnItems(List<Cell> cells, Random random)
        {
            if (_spawnableItems.Items.Select(x => x.Count).Aggregate((x, y) => x + y) > cells.Count)
                throw new UnityException("Not enough empty cells to place all items.");

            GameObject items = SpawningExtensions.CreateGameObject("Items", transform);

            foreach (SpawnableItem spawnableItem in _spawnableItems.Items)
            {
                GameObject itemParent = SpawningExtensions.CreateGameObject(spawnableItem.Item.name, items.transform);

                for (int j = 0; j < spawnableItem.Count; j++)
                {
                    Cell cell = cells[random.Next(cells.Count)];
                    _spawningExtensions.PlacePrefab(spawnableItem.Item, cell, itemParent.transform);
                    cell.CannotAddItem = true;
                    cells.Remove(cell);
                }
            }
        }

        /// <summary>
        /// Places a tile at the specified coordinates in the world.
        /// </summary>
        /// <param name="x">The x-coordinates of the room.</param>
        /// <param name="z">The z-coordinates of the room.</param>
        /// <param name="tile">The tile that needs to be placed.</param>
        /// <param name="parent">The parent of the teleporter.</param>
        public void PlaceTile(int x, int z, Tile tile, Transform parent)
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
                new Vector3(x * _spawningExtensions.TileSizeX, 0, z * _spawningExtensions.TileSizeZ),
                Quaternion.Euler(0, tile.Facing.RotationDegrees(), 0),
                parent
            );
        }

        /// <summary>
        /// Place the doors for the given room in the world. Which doors need to be spawned is determined from the
        /// allowed directions of the room.
        /// </summary>
        /// <param name="x">The x-position of the room, in the grid.</param>
        /// <param name="z">The z-position of the room, in the grid.</param>
        /// <param name="room">The room for which the doors need to be spawned.</param>
        /// <param name="direction">The direction in which the door should be placed.</param>
        /// <param name="cell">The cell where the key is spawned.</param>
        /// <param name="parent">The parent of the teleporter.</param>
        public void PlaceDoorInDirection(int x, int z, Room room, Direction direction, Cell cell,
            Transform parent)
        {
            Vector3 roomPosition = new(x * _spawningExtensions.TileSizeX, 0, z * _spawningExtensions.TileSizeZ);

            // Calculate the distance from the room center to where a door should be placed
            float doorDistanceFromRoomCenter = _spawningExtensions.TileSizeX / 2f - _doorDepthExtent;

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
            GameObject instantiatedDoorPrefab = SpawningExtensions.PlacePrefab(_doorPrefab, doorPosition, doorRotation, parent);
            Door doorComponent = instantiatedDoorPrefab.GetComponentInChildren<Door>();

            cell.CannotAddItem = true;

            GameObject instantiatedKeyPrefab = _spawningExtensions.PlacePrefab(_keyPrefab, cell, doorRotation, parent);

            Key keyComponent = instantiatedKeyPrefab.GetComponentInChildren<Key>();
            keyComponent.Initialize(doorComponent.DoorId, doorComponent.Color);
        }

        /// <summary>
        /// Sets the player spawn point to a random room.
        /// </summary>
        /// <param name="playerSpawnCell">The cell where the player should spawn.</param>
        public void SetPlayerSpawn(Cell playerSpawnCell)
        {
            GameObject player = GameObject.FindWithTag("Player");

            if (player == null) throw new UnityException("No player was found.");

            Vector3 spawningPoint = _spawningExtensions.CenterOfCell(playerSpawnCell) + _playerHeightOffset;

            Rigidbody playerRigidbody = player.GetComponent<Rigidbody>();
            playerRigidbody.position = spawningPoint;

            GameObject respawnPoint = GameObject.FindWithTag("Respawn");

            if (respawnPoint == null) return;

            respawnPoint.transform.position = spawningPoint;
            Area respawnArea = respawnPoint.GetComponent<Area>();
            respawnArea.Bounds = new Bounds(spawningPoint, respawnArea.Bounds.extents);

            GameObject winPoint = GameObject.FindWithTag("Win");

            if (winPoint == null) return;

            winPoint.transform.position = spawningPoint;
            Area winArea = winPoint.GetComponent<Area>();
            winArea.Bounds = new Bounds(spawningPoint, winArea.Bounds.extents);

            // Set the colors of the start and end rooms.
            playerSpawnCell.Tile.GameObject.GetComponent<Renderer>().material.color = _startRoomMat.color;
        }
    }
}
