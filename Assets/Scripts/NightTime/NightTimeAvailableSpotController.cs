using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;

[ExecuteInEditMode]
public class NightTimeAvailableSpotController : MonoBehaviour
{
    [SerializeField] private LineRenderer lineRenderer = null;
    [SerializeField] private Transform verts = null;
    [SerializeField] private Material matSelected = null;
    [SerializeField] private Material matNotSelected = null;
    private Vector2Int gridPoint;
    public Vector2Int GridPoint => gridPoint;

    private void Update()
    {
        UpdateLineRenderer();
    }
    public void UpdateLineRenderer()
    {
        Vector3[] pos = new Vector3[16];
        pos[0] = verts.GetChild(0).position;
        pos[1] = verts.GetChild(1).position;
        pos[2] = verts.GetChild(2).position;
        pos[3] = verts.GetChild(3).position;
        pos[4] = verts.GetChild(0).position;
        pos[5] = verts.GetChild(4).position;
        pos[6] = verts.GetChild(5).position;
        pos[7] = verts.GetChild(6).position;
        pos[8] = verts.GetChild(7).position;
        pos[9] = verts.GetChild(4).position;
        pos[10] = verts.GetChild(5).position;
        pos[11] = verts.GetChild(1).position;
        pos[12] = verts.GetChild(2).position;
        pos[13] = verts.GetChild(6).position;
        pos[14] = verts.GetChild(7).position;
        pos[15] = verts.GetChild(3).position;
        lineRenderer.SetPositions(pos);
    }

    public void SetPosition(Vector3 pos, Vector2Int gridPosition)
    {
        this.transform.position = pos;
        gridPoint = gridPosition;
        lineRenderer.enabled = true;
    }

    public void UpdateLineRendererColor(bool isSelected)
    {
        lineRenderer.material = isSelected ? matSelected : matNotSelected;
    }
}
