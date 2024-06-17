using System;
using System.Collections.Generic;
using UnityEngine;

namespace LevelGeneration
{
    /// <summary>
    /// Represents the spawnable items.
    /// </summary>
    [CreateAssetMenu(fileName = "SpawnableItems", menuName = "ScriptableObjects/SpawnableItems", order = 1)]
    public class SpawnableItems : ScriptableObject
    {
        [field: SerializeField]
        public List<SpawnableItem> Items { get; set; }
    }

    [Serializable]
    public class SpawnableItem
    {
        public GameObject Item;

        public int Count;
    }
}
