using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(PlayerPrefFloatVariable))]
public class PlayerPrefFloatVariableEditor : Editor
{
    float valueToSet = 0;
    public override void OnInspectorGUI(){
        serializedObject.Update();
		EditorGUILayout.PropertyField(serializedObject.FindProperty("ID"));
		EditorGUILayout.PropertyField(serializedObject.FindProperty("value"));
        valueToSet = EditorGUILayout.FloatField("New value", valueToSet, GUILayout.ExpandWidth(false));
        if(GUILayout.Button("Save Value", GUILayout.ExpandWidth(false))){
            PlayerPrefFloatVariable myScript = (PlayerPrefFloatVariable)target;
            myScript.SetValue(valueToSet);
        }
        if(GUILayout.Button("Load value", GUILayout.ExpandWidth(false))){
            PlayerPrefFloatVariable myScript = (PlayerPrefFloatVariable)target;
            myScript.GetLatestValue();
        }
		serializedObject.ApplyModifiedProperties();
    }
}
