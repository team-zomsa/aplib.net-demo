using Assets.Scripts.Wfc;
using UnityEditor;
using UnityEngine;

namespace Editors
{
    [CustomEditor(typeof(GridPlacer))]
    public class GridPlacerEditor : Editor
    {
        /// <summary>
        /// <inheritdoc />
        /// </summary>
        public override void OnInspectorGUI()
        {
            SerializedObject gridPlacerSettings = serializedObject;

            SerializedProperty roomObjects = gridPlacerSettings.FindProperty("_roomObjects");
            SerializedProperty doorPrefab = gridPlacerSettings.FindProperty("_doorPrefab");
            SerializedProperty keyPrefab = gridPlacerSettings.FindProperty("_keyPrefab");
            SerializedProperty useSeed = gridPlacerSettings.FindProperty("_useSeed");
            SerializedProperty seed = gridPlacerSettings.FindProperty("_seed");
            SerializedProperty tileSizeX = gridPlacerSettings.FindProperty("_tileSizeX");
            SerializedProperty tileSizeZ = gridPlacerSettings.FindProperty("_tileSizeZ");
            SerializedProperty gridWidthX = gridPlacerSettings.FindProperty("_gridWidthX");
            SerializedProperty gridWidthZ = gridPlacerSettings.FindProperty("_gridWidthZ");
            SerializedProperty amountOfRooms = gridPlacerSettings.FindProperty("_amountOfRooms");
            SerializedProperty teleporterPrefab = gridPlacerSettings.FindProperty("_teleporterPrefab");

            roomObjects.objectReferenceValue = EditorGUILayout.ObjectField("Room objects", roomObjects.objectReferenceValue, typeof(RoomObjects), false);
            doorPrefab.objectReferenceValue = EditorGUILayout.ObjectField("Door prefab", doorPrefab.objectReferenceValue, typeof(GameObject), false);
            keyPrefab.objectReferenceValue = EditorGUILayout.ObjectField("Key prefab", keyPrefab.objectReferenceValue, typeof(GameObject), false);
            teleporterPrefab.objectReferenceValue = EditorGUILayout.ObjectField("Teleporter Prefab", teleporterPrefab.objectReferenceValue, typeof(GameObject), false);
            useSeed.boolValue = EditorGUILayout.Toggle("Use seed", useSeed.boolValue);
            if (useSeed.boolValue) seed.intValue = EditorGUILayout.IntField("Seed", seed.intValue);
            tileSizeX.intValue = EditorGUILayout.IntField("Tile size X", tileSizeX.intValue);
            tileSizeZ.intValue = EditorGUILayout.IntField("Tile size Z", tileSizeZ.intValue);
            gridWidthX.intValue = EditorGUILayout.IntField("Grid Width X", gridWidthX.intValue);
            gridWidthZ.intValue = EditorGUILayout.IntField("Grid Width Z", gridWidthZ.intValue);
            amountOfRooms.intValue = EditorGUILayout.IntField("Amount of Rooms", amountOfRooms.intValue);

            gridPlacerSettings.ApplyModifiedProperties();

            if (GUILayout.Button("Generate new level"))
            {
                GridPlacer gridPlacer = (GridPlacer)target;
                foreach (Transform child in gridPlacer.transform) Destroy(child.gameObject);

                gridPlacer.MakeScene();
            }
        }
    }
}
