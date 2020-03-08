using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(PlayerPrefStringVariable))]
public class PlayerPrefStringVariableEditor : Editor
{
    string valueToSet = "";
    public override void OnInspectorGUI(){
        serializedObject.Update();
		EditorGUILayout.PropertyField(serializedObject.FindProperty("ID"));
		EditorGUILayout.PropertyField(serializedObject.FindProperty("value"));
        valueToSet = EditorGUILayout.TextField("New value", valueToSet, GUILayout.ExpandWidth(false));
        if(GUILayout.Button("Save Value", GUILayout.ExpandWidth(false))){
            PlayerPrefStringVariable myScript = (PlayerPrefStringVariable)target;
            myScript.SetValue(valueToSet);
        }
        if(GUILayout.Button("Load value", GUILayout.ExpandWidth(false))){
            PlayerPrefStringVariable myScript = (PlayerPrefStringVariable)target;
            myScript.Load();
        }
		serializedObject.ApplyModifiedProperties();
    }
}
