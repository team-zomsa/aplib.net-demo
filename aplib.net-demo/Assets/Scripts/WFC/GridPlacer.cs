using Assets.Scripts.Tiles;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.WFC
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
        /// The rotation of the tile.
        /// </summary>
        private const int _tileRotation = 90;

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
        /// Awake is called when the script instance is being loaded.
        /// </summary>
        public void Awake()
        {
            _grid = new Grid(5, 5);

            _grid.Init();

            TempFillFunction();

            for (int y = 0; y < _grid.Height; y++)
                for (int x = 0; x < _grid.Width; x++)
                    PlaceTile(x, y, _grid[x, y].Tile);
        }

        /// <summary>
        /// A temporary function to fill the grid with rooms.
        /// </summary>
        private void TempFillFunction()
        {
            _grid.PlaceRoom(2, 1, new Room(new List<bool> { false, true, true, false }));

            _grid.PlaceRoom(4, 4, new Room(new List<bool> { false, true, true, false }));

            // Road 1
            _grid[2, 2].Tile = new Straight();
            _grid[2, 3].Tile = new Straight();
            _grid[2, 4].Tile = new Corner(2);
            _grid[3, 4].Tile = new Straight(1);

            // Road 2
            _grid[3, 1].Tile = new Straight(1);
            _grid[4, 1].Tile = new TSection(3);
            _grid[4, 2].Tile = new Straight();
            _grid[4, 3].Tile = new Straight();
            _grid[4, 0].Tile = new DeadEnd();
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

            _ = Instantiate
            (
                prefab,
                new Vector3(x * _tileSizeX, 0, y * _tileSizeY),
                Quaternion.Euler(0, tile.Rotation * _tileRotation, 0),
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
            for (int direction = 0; direction <= 3; direction++)
            {
                // Continue to next direction if no door needs to be placed
                if (room.CanConnectInDirection(direction)) continue;

                Vector3 roomPosition = new(x * _tileSizeX, 0, y * _tileSizeY);
                float doorDepthExtend = _doorPrefab.GetComponent<Renderer>().bounds.extents.z;
                float doorDistanceFromRoomCenter = (_tileSizeX / 2f) - doorDepthExtend;
                Quaternion roomRotation = Quaternion.Euler(0, room.Rotation * _tileRotation, 0);
                (Vector3 relativeDoorPosition, Quaternion relativeDoorRotation) = direction switch
                {
                    0 => (new Vector3(-doorDistanceFromRoomCenter, 0, 0), Quaternion.Euler(0, -_tileRotation, 0)),
                    1 => (new Vector3(0, 0, doorDistanceFromRoomCenter), Quaternion.identity),
                    2 => (new Vector3(doorDistanceFromRoomCenter, 0, 0), Quaternion.Euler(0, _tileRotation, 0)),
                    3 => (new Vector3(0, 0, -doorDistanceFromRoomCenter), Quaternion.Euler(0, 2f * _tileRotation, 0)),
                    _ => throw new UnityException("Invalid direction when placing door")
                };
                Vector3 doorPosition = roomPosition + relativeDoorPosition;
                Quaternion doorRotation = roomRotation * relativeDoorRotation;

                _ = Instantiate(_doorPrefab, doorPosition, doorRotation, transform);
            }
        }
    }
}
