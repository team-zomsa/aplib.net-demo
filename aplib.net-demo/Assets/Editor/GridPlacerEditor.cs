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
            GridPlacer gridPlacer = (GridPlacer)target;
            gridPlacer.roomObjects = (RoomObjects)EditorGUILayout.ObjectField("Room objects", gridPlacer.roomObjects, typeof(RoomObjects), false);
            gridPlacer.tileSizeX = EditorGUILayout.IntField("Tile size X", gridPlacer.tileSizeX);
            gridPlacer.tileSizeZ = EditorGUILayout.IntField("Tile size Z", gridPlacer.tileSizeZ);
            gridPlacer.gridWidthX = EditorGUILayout.IntField("Grid Width X", gridPlacer.gridWidthX);
            gridPlacer.gridWidthZ = EditorGUILayout.IntField("Grid Width Z", gridPlacer.gridWidthZ);
            gridPlacer.amountOfRooms = EditorGUILayout.IntField("Amount of Rooms", gridPlacer.amountOfRooms);

            if (GUILayout.Button("Update Scene"))
                gridPlacer.UpdateScene();
        }
    }
}
