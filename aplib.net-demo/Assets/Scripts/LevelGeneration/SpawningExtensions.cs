// This program has been developed by students from the bachelor Computer Science at Utrecht
// University within the Software Project course.
// Copyright Utrecht University (Department of Information and Computing Sciences)

using UnityEngine;
using WFC;

namespace LevelGeneration
{
    /// <summary>
    /// Represents the spawnable extensions.
    /// </summary>
    public class SpawningExtensions : MonoBehaviour
    {
        /// <summary>
        /// The size of the tiles in the x-direction.
        /// </summary>
        [field: SerializeField]
        public int TileSizeX { get; set; }

        /// <summary>
        /// The size of the tiles in the z-direction.
        /// </summary>
        [field: SerializeField]
        public int TileSizeZ { get; set; }

        /// <summary>
        /// The offset of the floor.
        /// </summary>
        private readonly Vector3 _floorOffset = Vector3.up;

        /// <summary>
        /// Gets the cell coordinates of a given position.
        /// </summary>
        /// <param name="position">The position to get the cell coordinates for.</param>
        /// <returns>The cell coordinates of the given position.</returns>
        public (int x, int z) GetCellCoordinates(Vector3 position) =>
            ((int)(position.x / TileSizeX), (int)(position.z / TileSizeZ));

        /// <summary>
        /// Calculates the center of a cell's floor in world coordinates.
        /// </summary>
        /// <param name="cell">The cell to calculate its center of.</param>
        /// <returns>The real-world coordinates of the center of the cell's floor.</returns>
        public Vector3 CenterOfCell(Cell cell) => new(cell.X * TileSizeX, 0, cell.Z * TileSizeZ);

        /// <summary>
        /// Spawn an item in the given cell.
        /// </summary>
        /// <param name="prefab">The item prefab to spawn.</param>
        /// <param name="cell">The cell to spawn the item in.</param>
        /// <param name="parent">The parent of the item.</param>
        public void PlacePrefab(GameObject prefab, Cell cell, Transform parent) =>
            Instantiate(prefab, CenterOfCell(cell) + _floorOffset, Quaternion.identity, parent);

        /// <summary>
        /// Spawn an item in the given cell.
        /// </summary>
        /// <param name="prefab">The item prefab to spawn.</param>
        /// <param name="cell">The cell to spawn the item in.</param>
        /// <param name="rotation">The rotation of the item.</param>
        /// <param name="parent">The parent of the item.</param>
        public GameObject PlacePrefab(GameObject prefab, Cell cell, Quaternion rotation, Transform parent) =>
            Instantiate(prefab, CenterOfCell(cell) + _floorOffset, rotation, parent);

        /// <summary>
        /// Spawn an item in the given cell.
        /// </summary>
        /// <param name="prefab">The item prefab to spawn.</param>
        /// <param name="position">The position where the item is spawned.</param>
        /// <param name="rotation">The rotation of the item.</param>
        /// <param name="parent">The parent of the item.</param>
        public static GameObject PlacePrefab(GameObject prefab, Vector3 position, Quaternion rotation, Transform parent) =>
            Instantiate(prefab, position, rotation, parent);

        /// <summary>
        /// Creates a new game object with a given name and parent.
        /// </summary>
        /// <param name="objectName">The name of the game object.</param>
        /// <param name="parent">The parent of the game object.</param>
        /// <returns>The newly created game object.</returns>
        public static GameObject CreateGameObject(string objectName, Transform parent) =>
            new(objectName) { transform = { parent = parent } };
    }
}
