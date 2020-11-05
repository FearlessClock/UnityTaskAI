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
            fireController.StartFire();
        }
    }
}
