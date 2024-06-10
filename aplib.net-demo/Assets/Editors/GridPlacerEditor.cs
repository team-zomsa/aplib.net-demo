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

            SerializedProperty gridConfig = gridPlacerSettings.FindProperty("_gridConfig");

            gridConfig.objectReferenceValue =
                EditorGUILayout.ObjectField("Grid Config", gridConfig.objectReferenceValue, typeof(GridConfig), false);

            gridPlacerSettings.ApplyModifiedProperties();

            if (!GUILayout.Button("Generate new level") || !Application.IsPlaying(target) || CanvasManager.Instance.IsOnSettings ||
                CanvasManager.Instance.IsOnHelp) return;

            GridPlacer gridPlacer = (GridPlacer)target;
            foreach (Transform child in gridPlacer.transform) Destroy(child.gameObject);

            gridPlacer.WaitBeforeMakeScene();
        }
    }
}
