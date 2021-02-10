using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Generate2DColliderFromPoints))]
public class Generate2DColliderEditor : Editor
{
    Generate2DColliderFromPoints generateColliderFromPoints = null;

    private void OnEnable()
    {
        generateColliderFromPoints = (Generate2DColliderFromPoints)target;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (GUILayout.Button("Generate shape"))
        {
            generateColliderFromPoints.UpdatePolyCollider();
            EditorUtility.SetDirty(generateColliderFromPoints.gameObject);
        }
    }
}
