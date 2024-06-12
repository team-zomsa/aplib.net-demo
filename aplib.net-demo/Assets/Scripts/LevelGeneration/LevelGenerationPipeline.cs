using System;
using System.Collections;
using ThreadSafeRandom;
using UnityEngine;
using WFC;
using Grid = WFC.Grid;

namespace LevelGeneration
{
    [RequireComponent(typeof(LevelSpawner))]
    [RequireComponent(typeof(GameObjectPlacer))]
    [RequireComponent(typeof(EnemySpawner))]
    public class LevelGenerationPipeline : MonoBehaviour
    {
        /// <summary>
        /// Represents the grid config.
        /// </summary>
        [SerializeField]
        private GridConfig _gridConfig;

        /// <summary>
        /// The material of the start room.
        /// </summary>
        [SerializeField]
        private Material _startRoomMat;

        /// <summary>
        /// Represents the enemy spawner.
        /// </summary>
        private EnemySpawner _enemySpawner;

        /// <summary>
        /// Represents the game object placer.
        /// </summary>
        private GameObjectPlacer _gameObjectPlacer;

        /// <summary>
        /// Represents the level spawner.
        /// </summary>
        private LevelSpawner _levelSpawner;

        /// <summary>
        /// Represents the grid.
        /// </summary>
        public Grid Grid { get; private set; }

        public void Awake()
        {
            if (_gridConfig.UseSeed) SharedRandom.SetSeed(_gridConfig.Seed);

            Debug.Log($"Seed: {SharedRandom.Seed()}");

            _levelSpawner = GetComponent<LevelSpawner>();

            _gameObjectPlacer = GetComponent<GameObjectPlacer>();
            _gameObjectPlacer.Initialize();

            _enemySpawner = GetComponent<EnemySpawner>();

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
            if (_gridConfig.AmountOfRooms > _gridConfig.GridWidthX * _gridConfig.GridWidthZ)
                throw new Exception("The amount of rooms is larger than the available places in the grid.");

            if (_gridConfig.UseSeed) SharedRandom.SetSeed(_gridConfig.Seed);

            Debug.Log("Seed: " + SharedRandom.Seed());

            Grid = WaveFunctionCollapse.GenerateGrid(_gridConfig.GridWidthX, _gridConfig.GridWidthZ, _gridConfig.AmountOfRooms);
            _levelSpawner.Grid = Grid;

            Cell randomPlayerSpawn = Grid.GetRandomFilledCell();

            _levelSpawner.MakeScene(randomPlayerSpawn);

            // Set the colors of the start and end rooms.
            randomPlayerSpawn.Tile.GameObject.GetComponent<Renderer>().material.color = _startRoomMat.color;

            _gameObjectPlacer.SetPlayerSpawn(randomPlayerSpawn);

            _gameObjectPlacer.SpawnItems(Grid.GetAllCellsNotContainingItems());

            SpawnEnemies();
        }

        /// <summary>
        /// Spawns the enemies.
        /// </summary>
        public void SpawnEnemies() => _enemySpawner.SpawnEnemies(Grid.GetAllNotEmptyTiles());
    }
}
