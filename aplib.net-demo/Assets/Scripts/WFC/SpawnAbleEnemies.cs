using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Wfc
{
    /// <summary>
    /// Represents the spawnable enemies.
    /// </summary>
    [CreateAssetMenu(fileName = "SpawnableEnemies", menuName = "ScriptableObjects/SpawnableEnemies", order = 1)]
    public class SpawnableEnemies : ScriptableObject
    {
        [field: SerializeField]
        public List<SpawnableEnemy> Items { get; set; }
    }

    [Serializable]
    public class SpawnableEnemy
    {
        public GameObject Enemy;

        public int Count;
    }
}
