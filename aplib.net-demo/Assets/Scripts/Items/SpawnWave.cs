using Assets.Scripts.Wfc;
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
