using Assets.Scripts.Wfc;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = System.Random;

public class EnemySpawner : MonoBehaviour
{
    /// <summary>
    /// Represents the spawnable items.
    /// </summary>
    [SerializeField]
    private SpawnableEnemies _spawnableEnemies;

    /// <summary>
    /// Represents the spawning extensions.
    /// </summary>
    private SpawningExtensions _spawningExtensions;

    public void Initialize()
    {
        if (!TryGetComponent(out _spawningExtensions))
            throw new UnityException("SpawningExtensions not found.");
    }

    /// <summary>
    /// Spawns all items in the world.
    /// </summary>
    /// <param name="cells">The cells to spawn the items in.</param>
    /// <param name="random">The random number generator to use.</param>
    /// <exception cref="UnityException">Thrown when there are not enough empty cells to place all items.</exception>
    public void SpawnEnemies(List<Cell> cells, Random random)
    {
        if (_spawnableEnemies.Enemies.Select(x => x.Count).Sum() > cells.Count)
            throw new UnityException("Not enough empty cells to place all enemies.");

        GameObject enemies = SpawningExtensions.CreateGameObject("Enemies", transform);

        foreach (SpawnableEnemy spawnableEnemy in _spawnableEnemies.Enemies)
        {
            GameObject enemyParent = SpawningExtensions.CreateGameObject(spawnableEnemy.Enemy.name, enemies.transform);

            for (int j = 0; j < spawnableEnemy.Count; j++)
            {
                Cell cell = cells[random.Next(cells.Count)];
                _spawningExtensions.PlacePrefab(spawnableEnemy.Enemy, cell, enemyParent.transform);
                cells.Remove(cell);
            }
        }
    }
}
