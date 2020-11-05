using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

[CustomEditor(typeof(EntrancePoints))]
public class EntrancePointsEditor : Editor
{
    EntrancePoints entrancePoints = null;
    private void OnEnable()
    {
        entrancePoints = (EntrancePoints)target;
    }
    public override void OnInspectorGUI()
    {

        if (GUILayout.Button("Update Generator Values"))
        {
            entrancePoints.UpdateEntranceValues();

            EditorUtility.SetDirty(entrancePoints);
            EditorSceneManager.MarkSceneDirty(entrancePoints.gameObject.scene);
        }
        DrawDefaultInspector();
    }
}
