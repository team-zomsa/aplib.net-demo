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
        /// Represents the grid.
        /// </summary>
        private Grid _grid;

        /// <summary>
        /// Represents the room objects.
        /// </summary>
        public RoomObjects RoomObjects;

        /// <summary>
        /// Awake is called when the script instance is being loaded.
        /// </summary>
        public void Awake()
        {
            _grid = new Grid(5, 5);

            _grid.Init();
            
            TempFillFunction();

            for (int y = 0; y < _grid.Height; y++)
            {
                for (int x = 0; x < _grid.Width; x++)
                {
                    PlaceTile(x, y, _grid[x, y].Tile);
                }
            }
        }

        /// <summary>
        /// A temporary function to fill the grid with rooms.
        /// </summary>
        public void TempFillFunction()
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
        public void PlaceTile(int x, int y, Tile tile)
        {
            GameObject prefab = null;
            switch (tile)
            {
                case Corner _:
                    prefab = RoomObjects.Corner;
                    break;
                case Crossing _:
                    prefab = RoomObjects.Crossing;
                    break;
                case DeadEnd _:
                    prefab = RoomObjects.DeadEnd;
                    break;
                case Empty _:
                    prefab = RoomObjects.Empty;
                    break;
                case Room _:
                    prefab = RoomObjects.Room;
                    break;
                case Straight _:
                    prefab = RoomObjects.Straight;
                    break;
                case TSection _:
                    prefab = RoomObjects.TSection;
                    break;
            }

            if (prefab != null)
            {
                _ = Instantiate(prefab, new Vector3(x * 16, 0, y * 16), Quaternion.Euler(0, tile.Rotation * 90, 0), transform);
            }
        }
    }
}
