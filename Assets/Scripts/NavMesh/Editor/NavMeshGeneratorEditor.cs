using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Pieter.NavMesh
{
    [CustomEditor(typeof(Pieter.NavMesh.NavMeshGenerator))]
    public class NavMeshGeneratorEditor : Editor
    {
        NavMeshGenerator navMesh = null;
        SerializedProperty triangles;
        SerializedProperty entrancePoints;
        NavMeshTriangle selectedTriangle;
        int currentVertex = 0;
        [SerializeField] bool showButton = false;
        [SerializeField] bool showPosition = false;
        private void OnEnable()
        {
            navMesh = (NavMeshGenerator)target;
            triangles = serializedObject.FindProperty("triangles");
            entrancePoints = serializedObject.FindProperty("entrancePoints");
        }
        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            if(GUILayout.Button("Update Generator Values"))
            {
                navMesh.UpdateInformation();
            }
            SerializedProperty prop = serializedObject.FindProperty("m_Script");
            EditorGUILayout.PropertyField(prop, true, new GUILayoutOption[0]);
            showPosition = EditorGUILayout.Foldout(showPosition, "triangles");
            if (showPosition)
            {
                GUIContent ShowButtonGUIContent = new GUIContent("Show Buttons");
                showButton = EditorGUILayout.Toggle(ShowButtonGUIContent, showButton);
                EditorGUI.indentLevel++;
                for (int i = 0; i < navMesh.Triangles.Length; i++)
                {
                    SerializedProperty triangle = triangles.GetArrayElementAtIndex(i);

                    GUIContent gUIContent = new GUIContent(triangle.FindPropertyRelative("ID").intValue + (triangle.FindPropertyRelative("isSelected").boolValue ? " Selected" : ""));
                    EditorGUILayout.PropertyField(triangle, gUIContent, true); // draw property with it's children
                    if (showButton && GUILayout.Button("Select Vertex"))
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
                if (GUILayout.Button("Insert Triangle"))
                {
                    navMesh.AddNewTriangle();
                }
            }
            ////EditorGUILayout.LabelField(Selection.activeGameObject.name);
            //EditorGUILayout.Space();
            //DrawDefaultInspector();

            EditorGUILayout.PropertyField(entrancePoints, true);

            serializedObject.ApplyModifiedProperties();
        }

        private void OnSceneGUI()
        {
            HandleKeyboard();
        }

        private void HandleKeyboard()
        {
            Event current = Event.current;
            if (current.type != EventType.KeyDown)
                return;

            EditorGUI.BeginChangeCheck();
            switch (current.keyCode)
            {
                case KeyCode.DownArrow:
                    switch (currentVertex)
                    {
                        case 0:
                            selectedTriangle.vertex1 = Selection.activeGameObject.GetComponent<Vertex>();
                            break;
                        case 1:
                            selectedTriangle.vertex2 = Selection.activeGameObject.GetComponent<Vertex>();
                            break;
                        case 2:
                            selectedTriangle.vertex3 = Selection.activeGameObject.GetComponent<Vertex>();
                            break;
                        default:
                            break;
                    }
                    currentVertex++;
                    if(currentVertex >= 3)
                    {
                        currentVertex = 0;
                    }
                    current.Use();
                    break;
                case KeyCode.UpArrow:
                    currentVertex = 0;
                    if(selectedTriangle != null)
                    {
                        selectedTriangle.isSelected = false;
                    }
                    selectedTriangle = navMesh.AddNewTriangle();
                    selectedTriangle.isSelected = true;
                    current.Use();
                    break;
                case KeyCode.Return:
                case KeyCode.KeypadEnter:
                    current.Use();
                    break;
                case KeyCode.LeftArrow:
                case KeyCode.Backspace:
                    //current.Use();
                    break;
                case KeyCode.RightArrow:
                    //current.Use();
                    break;
                case KeyCode.Escape:
                    current.Use();
                    break;
            }
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(target, "Changed triangle Vertex");
            }
        }
    }
}
