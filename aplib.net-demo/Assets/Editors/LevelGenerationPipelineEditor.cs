using Assets.Scripts.Wfc;
using LevelGeneration;
using UnityEditor;
using UnityEngine;

namespace Editors
{
    [CustomEditor(typeof(LevelGenerationPipeline))]
    public class LevelGenerationEditor : Editor
    {
        /// <summary>
        /// <inheritdoc />
        /// </summary>
        public override void OnInspectorGUI()
        {
            SerializedObject levelGenerationPipelineSettings = serializedObject;

            SerializedProperty gridConfig = levelGenerationPipelineSettings.FindProperty("_gridConfig");
            SerializedProperty startRoomMaterial = levelGenerationPipelineSettings.FindProperty("_startRoomMat");

            gridConfig.objectReferenceValue =
                EditorGUILayout.ObjectField("Grid Config", gridConfig.objectReferenceValue, typeof(GridConfig), false);

            startRoomMaterial.objectReferenceValue = EditorGUILayout.ObjectField("Start Room Material",
                startRoomMaterial.objectReferenceValue, typeof(Material), false);

            levelGenerationPipelineSettings.ApplyModifiedProperties();

            if (!GUILayout.Button("Generate new level") || !Application.IsPlaying(target) || CanvasManager.Instance.IsOnSettings ||
                CanvasManager.Instance.IsOnHelp) return;

            LevelGenerationPipeline levelGenerationPipeline = (LevelGenerationPipeline)target;
            foreach (Transform child in levelGenerationPipeline.transform) Destroy(child.gameObject);

            levelGenerationPipeline.WaitBeforeMakeScene();
        }
    }
}