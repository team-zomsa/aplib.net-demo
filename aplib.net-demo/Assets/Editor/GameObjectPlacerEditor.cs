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

            SerializedProperty tileSizeX = gameObjectPlacerSettings.FindProperty("_tileSizeX");
            SerializedProperty tileSizeZ = gameObjectPlacerSettings.FindProperty("_tileSizeZ");
            SerializedProperty roomObjects = gameObjectPlacerSettings.FindProperty("_roomObjects");
            SerializedProperty doorPrefab = gameObjectPlacerSettings.FindProperty("_doorPrefab");
            SerializedProperty keyPrefab = gameObjectPlacerSettings.FindProperty("_keyPrefab");
            SerializedProperty teleporterPrefab = gameObjectPlacerSettings.FindProperty("_teleporterPrefab");
            SerializedProperty endItemPrefab = gameObjectPlacerSettings.FindProperty("_endItemPrefab");

            tileSizeX.intValue = EditorGUILayout.IntField("Tile size X", tileSizeX.intValue);
            tileSizeZ.intValue = EditorGUILayout.IntField("Tile size Z", tileSizeZ.intValue);
            roomObjects.objectReferenceValue = EditorGUILayout.ObjectField("Room objects",
                roomObjects.objectReferenceValue, typeof(RoomObjects), false);
            doorPrefab.objectReferenceValue = EditorGUILayout.ObjectField("Door prefab",
                doorPrefab.objectReferenceValue, typeof(GameObject), false);
            keyPrefab.objectReferenceValue = EditorGUILayout.ObjectField("Key prefab", keyPrefab.objectReferenceValue,
                typeof(GameObject), false);
            teleporterPrefab.objectReferenceValue = EditorGUILayout.ObjectField("Teleporter Prefab",
                teleporterPrefab.objectReferenceValue, typeof(GameObject), false);
            endItemPrefab.objectReferenceValue = EditorGUILayout.ObjectField("End Item Prefab",
                endItemPrefab.objectReferenceValue, typeof(GameObject), false);

            gameObjectPlacerSettings.ApplyModifiedProperties();
        }
    }
}
