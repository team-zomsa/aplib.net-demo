// This program has been developed by students from the bachelor Computer Science at Utrecht
// University within the Software Project course.
// Copyright Utrecht University (Department of Information and Computing Sciences)

using System.Collections.Generic;
using System.Linq;
using ThreadSafeRandom;
using UnityEngine;
using WFC;

namespace LevelGeneration
{
    [RequireComponent(typeof(SpawningExtensions))]
    public class EnemySpawner : MonoBehaviour
    {
        /// <summary>
        /// Represents the spawnable enemies.
        /// </summary>
        [SerializeField]
        private SpawnableEnemies _spawnableEnemies;

        /// <summary>
        /// Represents the spawning extensions.
        /// </summary>
        private SpawningExtensions _spawningExtensions;

        public void Awake() => _spawningExtensions = GetComponent<SpawningExtensions>();

        /// <summary>
        /// Spawns all enemies in the world.
        /// </summary>
        /// <param name="cells">The cells to spawn the enemies in.</param>
        /// <exception cref="UnityException">Thrown when there are not enough empty cells to place all items.</exception>
        public void SpawnEnemies(List<Cell> cells)
        {
            if (_spawnableEnemies.Enemies.Select(x => x.Count).Sum() > cells.Count)
                throw new UnityException("Not enough empty cells to place all enemies.");

            GameObject enemies = GameObject.Find("Enemies");

            if (enemies == null) enemies = SpawningExtensions.CreateGameObject("Enemies", transform);

            foreach (SpawnableEnemy spawnableEnemy in _spawnableEnemies.Enemies)
            {
                GameObject enemyParent = GameObject.Find(spawnableEnemy.Enemy.name);

                if (enemyParent == null) enemyParent = SpawningExtensions.CreateGameObject(spawnableEnemy.Enemy.name, enemies.transform);

                for (int j = 0; j < spawnableEnemy.Count; j++)
                {
                    Cell cell = cells[SharedRandom.Next(cells.Count)];
                    _spawningExtensions.PlacePrefab(spawnableEnemy.Enemy, cell, enemyParent.transform);
                    cells.Remove(cell);
                }
            }
        }
    }
}
