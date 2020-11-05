using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(PersonAIDebugHolder))]
public class PersonAIDebugHolderEditor : Editor
{
    PersonAIDebugHolder personAIDebugHolder = null;
    private GUIStyle white = null;

    private void OnEnable()
    {
        personAIDebugHolder = (PersonAIDebugHolder)target;

        white = new GUIStyle(EditorStyles.label);
        white.normal.textColor = Color.white;
    }
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        for (int i = personAIDebugHolder.debugText.Count-1; i >= 0; i--)
        {
            GUI.contentColor = personAIDebugHolder.debugColor[i];
            GUILayout.Label(personAIDebugHolder.debugText[i], white);
        }
    }
}
