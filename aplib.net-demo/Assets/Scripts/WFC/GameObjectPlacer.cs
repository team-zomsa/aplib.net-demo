using Assets.Scripts.Doors;
using Assets.Scripts.Tiles;
using UnityEngine;
using static Assets.Scripts.Tiles.Direction;

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
        /// The `Renderer` component for the door prefab.
        /// </summary>
        /// <remarks>Getting a reference to the component is expensive, so we only want to do it once.</remarks>
        private Renderer _doorRenderer;

        /// <summary>
        /// This contains the whole 'pipeline' of level generation, including initialising the grid and placing teleporters.
        /// </summary>
        public void Awake() => _doorRenderer = _doorPrefab.GetComponent<Renderer>();

        /// <summary>
        /// Gets the cell coordinates of a given position.
        /// </summary>
        /// <param name="position">The position to get the cell coordinates for.</param>
        /// <returns>The cell coordinates of the given position.</returns>
        public (int x, int z) GetCellCoordinates(Vector3 position) =>
            ((int)(position.x / _tileSizeX), (int)(position.z / _tileSizeZ));

        /// <summary>
        /// Calculates the centre of a cell's floor in world coordinates.
        /// </summary>
        /// <param name="cell">The cell to calculate its centre of.</param>
        /// <returns>The real-world coordinates of the centre of the cell's floor.</returns>
        public Vector3 CentreOfCell(Cell cell) => new(cell.X * _tileSizeX, 0, cell.Z * _tileSizeZ);

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
        /// Spawn the end item in the given connected component.
        /// </summary>
        /// <param name="cell">The cell to spawn the end item in.</param>
        /// <param name="parent">The parent of the end item.</param>
        public void PlaceEndItem(Cell cell, Transform parent) =>
            Instantiate(_endItemPrefab, CentreOfCell(cell) + Vector3.up, Quaternion.identity, parent);

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
                new Vector3(x * _tileSizeX, 0, z * _tileSizeZ),
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
            Vector3 roomPosition = new(x * _tileSizeX, 0, z * _tileSizeZ);

            // Get (half of) the depth of the door model
            float doorDepthExtend = _doorRenderer.bounds.extents.z;

            // Calculate the distance from the room center to where a door should be placed
            float doorDistanceFromRoomCenter = (_tileSizeX / 2f) - doorDepthExtend;

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

            cell.CannotAddItem = true;

            GameObject instantiatedKeyPrefab =
                Instantiate(_keyPrefab, CentreOfCell(cell) + Vector3.up, doorRotation, parent);

            Key keyComponent = instantiatedKeyPrefab.GetComponentInChildren<Key>();
            keyComponent.Initialize(doorComponent.DoorId, doorComponent.Color);
        }
    }
}
