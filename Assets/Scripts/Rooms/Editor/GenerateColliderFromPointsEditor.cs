using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(GenerateColliderFromPoints))]
public class GenerateColliderFromPointsEditor : Editor
{
    GenerateColliderFromPoints generateColliderFromPoints = null;

    private void OnEnable()
    {
        generateColliderFromPoints = (GenerateColliderFromPoints)target;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (GUILayout.Button("Generate shape"))
        {
            Mesh mesh = generateColliderFromPoints.UpdatePolyCollider();
            string filePath =
                EditorUtility.SaveFilePanelInProject("Save Procedural Mesh", generateColliderFromPoints.transform.parent.parent.name + "Mesh", "asset", "");
            if (filePath == "") return;
            AssetDatabase.CreateAsset(mesh, filePath);
            EditorUtility.SetDirty(generateColliderFromPoints.gameObject);
        }
    }
}
