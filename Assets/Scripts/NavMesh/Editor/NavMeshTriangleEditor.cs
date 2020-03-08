using UnityEngine;
using UnityEditor;

namespace Pieter.NavMesh
{
    [CustomEditor(typeof(Pieter.NavMesh.NavMeshTriangle))]
    public class NavMeshTriangleEditor : Editor
    {
        //NavMeshTriangle navMeshTriangle = null;
        //SerializedProperty vertex1 = null;
        //private void OnEnable()
        //{
        //    navMeshTriangle = (NavMeshTriangle)target;
        //    vertex1 = serializedObject.FindProperty("vertex1");
        //}
        //public override void OnInspectorGUI()
        //{

        //    EditorGUILayout.PropertyField(vertex1, true); // draw property with it's children

        //    EditorGUILayout.Space();
        //}
    }
}