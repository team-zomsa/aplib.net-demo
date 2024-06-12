using Assets.Scripts.Wfc;
using System;
using System.Collections;
using ThreadSafeRandom;
using UnityEngine;
using Grid = Assets.Scripts.Wfc.Grid;

namespace LevelGeneration
{
    [RequireComponent(typeof(LevelSpawner))]
    [RequireComponent(typeof(GameObjectPlacer))]
    [RequireComponent(typeof(SpawningExtensions))]
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
        private Grid _grid { get; set; }

        public void Awake()
        {
            if (_gridConfig.UseSeed) SharedRandom.SetSeed(_gridConfig.Seed);

            Debug.Log($"Seed: {SharedRandom.Seed()}");

            _levelSpawner = GetComponent<LevelSpawner>();

            _gameObjectPlacer = GetComponent<GameObjectPlacer>();
            _gameObjectPlacer.Initialize();

            _enemySpawner = GetComponent<EnemySpawner>();
            _enemySpawner.Initialize();

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

            _grid = WaveFunctionCollapse.GenerateGrid(_gridConfig.GridWidthX, _gridConfig.GridWidthZ, _gridConfig.AmountOfRooms);
            _levelSpawner.Grid = _grid;

            Cell randomPlayerSpawn = _grid.GetRandomFilledCell();

            _levelSpawner.MakeScene(randomPlayerSpawn, _startRoomMat);

            _gameObjectPlacer.SetPlayerSpawn(randomPlayerSpawn);

            _gameObjectPlacer.SpawnItems(_grid.GetAllCellsNotContainingItems());

            SpawnEnemies();
        }

        public void SpawnEnemies() => _enemySpawner.SpawnEnemies(_grid.GetAllNotEmptyTiles());
    }
}
