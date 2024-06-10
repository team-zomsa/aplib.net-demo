using Assets.Scripts.Wfc;
using UnityEngine;

namespace Assets.Scripts.Items
{
    [RequireComponent(typeof(PickupableItem))]
    public class SpawnWave : MonoBehaviour
    {
        private GridPlacer _gridPlacer;

        private PickupableItem _item;

        private void Awake()
        {
            _item = GetComponent<PickupableItem>();
            _gridPlacer = FindObjectOfType<GridPlacer>();
        }

        private void OnEnable() => _item.ItemPickedUp += SpawnEnemyWave;

        private void OnDisable() => _item.ItemPickedUp -= SpawnEnemyWave;

        private void SpawnEnemyWave(Item _) => _gridPlacer.SpawnEnemies();
    }
}
