using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Wfc
{
    /// <summary>
    /// Represents the spawnable items.
    /// </summary>
    [CreateAssetMenu(fileName = "SpawnAbleItems", menuName = "ScriptableObjects/SpawnAbleItems", order = 1)]
    public class SpawnAbleItems : ScriptableObject
    {
        /// <summary>
        /// Represents the spawnable items count.
        /// </summary>
        public List<int> SpawnableItemsCount = new();

        /// <summary>
        /// Represents the spawnable items.
        /// </summary>
        public List<GameObject> SpawnableItems = new();
    }
}
