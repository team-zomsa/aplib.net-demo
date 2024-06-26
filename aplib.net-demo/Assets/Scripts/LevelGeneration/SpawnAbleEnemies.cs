// This program has been developed by students from the bachelor Computer Science at Utrecht
// University within the Software Project course.
// Copyright Utrecht University (Department of Information and Computing Sciences)

using System;
using System.Collections.Generic;
using UnityEngine;

namespace LevelGeneration
{
    /// <summary>
    /// Represents the spawnable enemies.
    /// </summary>
    [CreateAssetMenu(fileName = "SpawnableEnemies", menuName = "ScriptableObjects/SpawnableEnemies", order = 1)]
    public class SpawnableEnemies : ScriptableObject
    {
        [field: SerializeField]
        public List<SpawnableEnemy> Enemies { get; set; }
    }

    [Serializable]
    public class SpawnableEnemy
    {
        public GameObject Enemy;

        public int Count;
    }
}
