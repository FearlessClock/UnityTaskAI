using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(TaskListHolder))]
public class TaskHolderEditor : Editor
{
    TaskListHolder taskListHolder = null;
    SerializedProperty tasks;

    private void OnEnable()
    {
        taskListHolder = (TaskListHolder)target;
        tasks = serializedObject.FindProperty("tasks");
    }
    public override void OnInspectorGUI()
    {
        SerializedProperty prop = serializedObject.FindProperty("m_Script");
        EditorGUILayout.PropertyField(prop, true, new GUILayoutOption[0]);

        EditorGUILayout.TextField("Tasks");
        for (int i = 0; i < taskListHolder.Length; i++)
        {
            EditorGUILayout.TextField(taskListHolder.Tasks[0].GetTaskInformation);
        }
        //DrawDefaultInspector();
    }
}
