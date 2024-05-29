using Assets.Scripts.Wfc;
using UnityEditor;

namespace Editors
{
    [CustomEditor(typeof(EnemySpawner))]
    public class EnemySpawnerEditor : Editor
    {
        /// <summary>
        /// <inheritdoc />
        /// </summary>
        public override void OnInspectorGUI()
        {
            SerializedObject gameObjectPlacerSettings = serializedObject;

            SerializedProperty spawnAbleEnemies = gameObjectPlacerSettings.FindProperty("_spawnableEnemies");

            spawnAbleEnemies.objectReferenceValue = EditorGUILayout.ObjectField("Spawnable Enemies",
                spawnAbleEnemies.objectReferenceValue, typeof(SpawnableEnemies), false);

            gameObjectPlacerSettings.ApplyModifiedProperties();
        }
    }
}
