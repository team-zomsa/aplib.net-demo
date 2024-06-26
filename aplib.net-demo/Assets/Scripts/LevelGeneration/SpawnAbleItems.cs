// This program has been developed by students from the bachelor Computer Science at Utrecht
// University within the Software Project course.
// Copyright Utrecht University (Department of Information and Computing Sciences)

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
