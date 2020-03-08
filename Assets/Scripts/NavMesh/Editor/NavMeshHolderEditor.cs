using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Pieter.NavMesh
{
    [CustomEditor(typeof(Pieter.NavMesh.NavMeshHolder))]
    public class NavMeshHolderEditor : Editor
    {
        NavMeshHolder navMesh = null;
        SerializedProperty triangles;
        NavMeshTriangle selectedTriangle;
        int currentVertex = 0;
        [SerializeField] bool showPosition = false;
        private void OnEnable()
        {
            navMesh = (NavMeshHolder)target;
            triangles = serializedObject.FindProperty("triangles");
        }
        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            if(GUILayout.Button("Update Generator Values"))
            {
                //navMesh.UpdateInformation();
            }
            // Add script drawer
            SerializedProperty prop = serializedObject.FindProperty("m_Script");
            EditorGUILayout.PropertyField(prop, true, new GUILayoutOption[0]);

            showPosition = EditorGUILayout.Foldout(showPosition, "triangles");
            if (showPosition)
            {
                EditorGUI.indentLevel++;
                for (int i = 0; i < navMesh.Triangles.Length; i++)
                {
                    SerializedProperty triangle = triangles.GetArrayElementAtIndex(i);

                    GUIContent gUIContent = new GUIContent(triangle.FindPropertyRelative("ID").intValue + (triangle.FindPropertyRelative("isSelected").boolValue ? " Selected" : ""));
                    EditorGUILayout.PropertyField(triangle, gUIContent, true); // draw property with it's children
                    if (GUILayout.Button("Select Triangle"))
                    {
                        currentVertex = 0;
                        if (selectedTriangle != null)
                        {
                            selectedTriangle.isSelected = false;
                        }
                        selectedTriangle = navMesh.Triangles[i];
                        selectedTriangle.isSelected = true;
                    }
                }
                EditorGUI.indentLevel--;
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}
