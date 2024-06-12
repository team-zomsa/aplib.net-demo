using Assets.Scripts.Wfc;
using UnityEditor;
using UnityEngine;

namespace Editors
{
    [CustomEditor(typeof(GameObjectPlacer))]
    public class GameObjectPlacerEditor : Editor
    {
        /// <summary>
        /// <inheritdoc />
        /// </summary>
        public override void OnInspectorGUI()
        {
            SerializedObject gameObjectPlacerSettings = serializedObject;

            SerializedProperty roomObjects = gameObjectPlacerSettings.FindProperty("_roomObjects");
            SerializedProperty spawnAbleItems = gameObjectPlacerSettings.FindProperty("_spawnableItems");
            SerializedProperty doorPrefab = gameObjectPlacerSettings.FindProperty("_doorPrefab");
            SerializedProperty keyPrefab = gameObjectPlacerSettings.FindProperty("_keyPrefab");
            SerializedProperty teleporterPrefab = gameObjectPlacerSettings.FindProperty("_teleporterPrefab");
            SerializedProperty endItemPrefab = gameObjectPlacerSettings.FindProperty("_endItemPrefab");
            SerializedProperty endRoomMaterial = gameObjectPlacerSettings.FindProperty("_endRoomMat");

            roomObjects.objectReferenceValue = EditorGUILayout.ObjectField("Room objects",
                roomObjects.objectReferenceValue, typeof(RoomObjects), false);

            spawnAbleItems.objectReferenceValue = EditorGUILayout.ObjectField("Spawnable Items",
                spawnAbleItems.objectReferenceValue, typeof(SpawnableItems), false);

            doorPrefab.objectReferenceValue = EditorGUILayout.ObjectField("Door prefab",
                doorPrefab.objectReferenceValue, typeof(GameObject), false);

            keyPrefab.objectReferenceValue = EditorGUILayout.ObjectField("Key prefab", keyPrefab.objectReferenceValue,
                typeof(GameObject), false);

            teleporterPrefab.objectReferenceValue = EditorGUILayout.ObjectField("Teleporter prefab",
                teleporterPrefab.objectReferenceValue, typeof(GameObject), false);

            endItemPrefab.objectReferenceValue = EditorGUILayout.ObjectField("End Item prefab",
                endItemPrefab.objectReferenceValue, typeof(GameObject), false);

            endRoomMaterial.objectReferenceValue = EditorGUILayout.ObjectField("End Room Material",
                endRoomMaterial.objectReferenceValue, typeof(Material), false);

            gameObjectPlacerSettings.ApplyModifiedProperties();
        }
    }
}
