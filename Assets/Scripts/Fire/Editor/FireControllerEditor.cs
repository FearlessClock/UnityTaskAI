using Pieter.Grid;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(FireController))]
public class FireControllerEditor : Editor
{
    private FireController fireController = null;

    private void OnEnable()
    {
        fireController = (FireController)target;
    }
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        if (GUILayout.Button("Start Fire"))
        {
            RoomInformation room = fireController.GetRandomRoom();
            GridPoint point = room.roomGrid.GetRandomGridPoint();
            Vector2Int pos = new Vector2Int((int)point.gridPosition.x, (int)point.gridPosition.y);
            fireController.StartFire(pos, room);
        }
    }
}
