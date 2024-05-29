using Assets.Scripts.Wfc;
using UnityEditor;
using UnityEngine;

namespace Editor
{
    [CustomEditor(typeof(GameObjectPlacer))]
    public class GameObjectPlacerEditor : UnityEditor.Editor
    {
        /// <summary>
        /// <inheritdoc />
        /// </summary>
        public override void OnInspectorGUI()
        {
            SerializedObject gameObjectPlacerSettings = serializedObject;

            SerializedProperty tileSizeX = gameObjectPlacerSettings.FindProperty("_tileSizeX");
            SerializedProperty tileSizeZ = gameObjectPlacerSettings.FindProperty("_tileSizeZ");
            SerializedProperty roomObjects = gameObjectPlacerSettings.FindProperty("_roomObjects");
            SerializedProperty spawnAbleItems = gameObjectPlacerSettings.FindProperty("_spawnableItems");
            SerializedProperty doorPrefab = gameObjectPlacerSettings.FindProperty("_doorPrefab");
            SerializedProperty keyPrefab = gameObjectPlacerSettings.FindProperty("_keyPrefab");
            SerializedProperty teleporterPrefab = gameObjectPlacerSettings.FindProperty("_teleporterPrefab");
            SerializedProperty endItemPrefab = gameObjectPlacerSettings.FindProperty("_endItemPrefab");
            SerializedProperty startRoomMaterial = gameObjectPlacerSettings.FindProperty("_startRoomMat");
            SerializedProperty endRoomMaterial = gameObjectPlacerSettings.FindProperty("_endRoomMat");

            tileSizeX.intValue = EditorGUILayout.IntField("Tile size X", tileSizeX.intValue);
            tileSizeZ.intValue = EditorGUILayout.IntField("Tile size Z", tileSizeZ.intValue);
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

            startRoomMaterial.objectReferenceValue = EditorGUILayout.ObjectField("Start Room Material",
                startRoomMaterial.objectReferenceValue, typeof(Material), false);

            endRoomMaterial.objectReferenceValue = EditorGUILayout.ObjectField("End Room Material",
                endRoomMaterial.objectReferenceValue, typeof(Material), false);

            gameObjectPlacerSettings.ApplyModifiedProperties();
        }
    }
}
