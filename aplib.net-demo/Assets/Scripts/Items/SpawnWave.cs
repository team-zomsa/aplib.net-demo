// This program has been developed by students from the bachelor Computer Science at Utrecht
// University within the Software Project course.
// Copyright Utrecht University (Department of Information and Computing Sciences)

using LevelGeneration;
using UnityEngine;

namespace Assets.Scripts.Items
{
    [RequireComponent(typeof(PickupableItem))]
    public class SpawnWave : MonoBehaviour
    {
        private LevelGenerationPipeline _levelGenerationPipeline;

        private PickupableItem _item;

        private void Awake()
        {
            _item = GetComponent<PickupableItem>();
            _levelGenerationPipeline = FindObjectOfType<LevelGenerationPipeline>();
        }

        private void OnEnable() => _item.ItemPickedUp += SpawnEnemyWave;

        private void OnDisable() => _item.ItemPickedUp -= SpawnEnemyWave;

        private void SpawnEnemyWave(Item _) => _levelGenerationPipeline.SpawnEnemies();
    }
}
