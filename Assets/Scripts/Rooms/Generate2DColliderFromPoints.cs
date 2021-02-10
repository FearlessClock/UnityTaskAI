using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Generate2DColliderFromPoints : MonoBehaviour
{
    [SerializeField] private PolygonCollider2D polygonCollider = null;
    [SerializeField] private Transform parentCorners = null;

    public void UpdatePolyCollider()
    {
        Vector2[] points = new Vector2[parentCorners.childCount];
        for (int i = 0; i < parentCorners.childCount; i++)
        {
            points[i] = parentCorners.GetChild(i).transform.position;
        }
        polygonCollider.SetPath(0,points);
    }
}
