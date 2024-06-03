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

            SerializedProperty useSeed = gridPlacerSettings.FindProperty("_useSeed");
            SerializedProperty seed = gridPlacerSettings.FindProperty("_seed");
            SerializedProperty gridWidthX = gridPlacerSettings.FindProperty("_gridWidthX");
            SerializedProperty gridWidthZ = gridPlacerSettings.FindProperty("_gridWidthZ");
            SerializedProperty amountOfRooms = gridPlacerSettings.FindProperty("_amountOfRooms");

            useSeed.boolValue = EditorGUILayout.Toggle("Use seed", useSeed.boolValue);
            if (useSeed.boolValue) seed.intValue = EditorGUILayout.IntField("Seed", seed.intValue);
            gridWidthX.intValue = EditorGUILayout.IntField("Grid Width X", gridWidthX.intValue);
            gridWidthZ.intValue = EditorGUILayout.IntField("Grid Width Z", gridWidthZ.intValue);
            amountOfRooms.intValue = EditorGUILayout.IntField("Amount of Rooms", amountOfRooms.intValue);

            gridPlacerSettings.ApplyModifiedProperties();

            if (!GUILayout.Button("Generate new level") || !Application.IsPlaying(target) || CanvasManager.Instance.IsOnGameSettings ||
                CanvasManager.Instance.IsOnMenuSettings) return;

            GridPlacer gridPlacer = (GridPlacer)target;
            foreach (Transform child in gridPlacer.transform) Destroy(child.gameObject);

            gridPlacer.WaitBeforeMakeScene();
        }
    }
}
