using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


[CustomEditor(typeof(NightTimeAvailableSpotController))]
public class AvailableSpotControllerEditor : Editor
{
    NightTimeAvailableSpotController spotController = null;
    private void OnEnable()
    {
        spotController = (NightTimeAvailableSpotController)target;
    }

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        if (GUILayout.Button("Update Cage"))
        {
            spotController.UpdateLineRenderer();
        }
    }
}
