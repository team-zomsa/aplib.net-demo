using Assets.Scripts.Wfc;
using System;
using System.Collections;
using ThreadSafeRandom;
using UnityEngine;
using Grid = Assets.Scripts.Wfc.Grid;

namespace LevelGeneration
{
    public class LevelGenerationPipeline : MonoBehaviour
    {
        /// <summary>
        /// Represents the grid config.
        /// </summary>
        [SerializeField]
        private GridConfig _gridConfig;

        /// <summary>
        /// Represents the grid.
        /// </summary>
        public Grid Grid { get; private set; }

        public void Awake()
        {
            if (_gridConfig.UseSeed) SharedRandom.SetSeed(_gridConfig.Seed);

            Debug.Log($"Seed: {SharedRandom.Seed()}");

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

        private void MakeScene()
        {
            if (_gridConfig.AmountOfRooms > _gridConfig.GridWidthX * _gridConfig.GridWidthZ)
                throw new Exception("The amount of rooms is larger than the available places in the grid.");

            if (_gridConfig.UseSeed) SharedRandom.SetSeed(_gridConfig.Seed);

            Debug.Log("Seed: " + SharedRandom.Seed());

            Grid = WaveFunctionCollapse.GenerateGrid(_gridConfig.GridWidthX, _gridConfig.GridWidthZ, _gridConfig.AmountOfRooms);
        }
    }
}
