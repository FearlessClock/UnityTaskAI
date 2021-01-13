using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct PolyVertexInformation
{
    public int id;
    public Vector3 pos;

    public override string ToString()
    {
        return "Vertex " + id;
    }
}

public class GenerateColliderFromPoints : MonoBehaviour
{
    [SerializeField] private MeshCollider meshCollider = null;
    [SerializeField] private Transform parentCorners = null;

    public Mesh UpdatePolyCollider()
    {
        Mesh mesh = new Mesh();
        Vector3[] verts = new Vector3[parentCorners.childCount];
        for (int i = 0; i < parentCorners.childCount; i++)
        {
            verts[i] = parentCorners.GetChild(i).position;
        }

        mesh.vertices = verts;
        mesh.triangles = TrianglatePoly(verts);
        mesh.name = this.transform.parent.parent.name;
        meshCollider.sharedMesh = mesh;
        Vector2[] points = new Vector2[parentCorners.childCount];

        return mesh;
    }

    private int[] TrianglatePoly(Vector3[] verts)
    {
        int counter = 1000;
        List<int> triangles = new List<int>();
        List<PolyVertexInformation> listOfVerts = new List<PolyVertexInformation>();
        for (int i = 0; i < verts.Length; i++)
        {
            listOfVerts.Add(new PolyVertexInformation() { pos = verts[i], id = i });
        }
        while(listOfVerts.Count > 3 && counter > 0)
        {
            int res = FindEar(listOfVerts.ToArray());
            if (res >= 0)
            {
                triangles.AddRange(GetTriVertexes(res, listOfVerts));
                for (int i = 0; i < listOfVerts.Count; i++)
                {
                    if(listOfVerts[i].id == res)
                    {
                        listOfVerts.RemoveAt(i);
                        break;
                    }
                }
            }
            counter--;
        }
        for (int i = 0; i < listOfVerts.Count; i++)
        {
            triangles.Add(listOfVerts[i].id);
        }
        return triangles.ToArray();
    }

    private int[] GetTriVertexes(int id, List<PolyVertexInformation> vertexes)
    {
        int index1 = GetIndexFromId(id, vertexes.ToArray());
        int index0 = index1 - 1;
        if (index0 == -1)
        {
            index0 = vertexes.Count - 1;
        }
        int index2 = index1 + 1;
        if (index2 == vertexes.Count)
        {
            index2 = 0;
        }

        int[] indexes = new int[3] { vertexes[index0].id, id, vertexes[index2].id };
        return indexes;
    }

    // In 2D
    private int FindEar(PolyVertexInformation[] verts)
    {
        int i = 0;
        bool earNotFound = true;
        while (earNotFound)
        {
            if (CheckConvexFor(i, verts))
            {
                bool hasContainingVertex = false;
                for (int j = 0; j < verts.Length; j++)
                {
                    if ((j != i - 1 && j != i && j != i + 1) && !CheckConvexFor(j, verts) &&
                        IsPositionInTriangle(verts[j].pos, GetVertAt(i - 1, verts), GetVertAt(i, verts), GetVertAt(i + 1, verts)))
                    {
                        hasContainingVertex = true;
                        break;
                    }
                }

                if (!hasContainingVertex)
                {
                    Debug.Log(verts[i].id);
                    earNotFound = false;
                }
            }
            if (earNotFound)
            {
                i++;
                if (i > verts.Length - 1)
                {
                    return -1;
                }
            }
        }
        return verts[i].id;
    }

    private Vector3 GetVertAt(int index, PolyVertexInformation[] verts)
    {
        if (index == -1)
        {
            index = verts.Length - 1;
        }
        else if (index == verts.Length)
        {
            index = 0;
        }
        return verts[index].pos;
    }

    private bool IsPositionInTriangle(Vector3 pos, Vector3 p0, Vector3 p1, Vector3 p2)
    {
        // Compute vectors        
        Vector3 v0 = p1 - p0;
        Vector3 v1 = p2 - p0;
        Vector3 v2 = pos - p0;

        // Compute dot products
        float dot00 = Vector3.Dot(v0, v0);
        float dot01 = Vector3.Dot(v0, v1);
        float dot02 = Vector3.Dot(v0, v2);
        float dot11 = Vector3.Dot(v1, v1);
        float dot12 = Vector3.Dot(v1, v2);

        // Compute barycentric coordinates
        float invDenom = 1 / (dot00 * dot11 - dot01 * dot01);
        float u = (dot11 * dot02 - dot01 * dot12) * invDenom;
        float v = (dot00 * dot12 - dot01 * dot02) * invDenom;

        // Check if point is in triangle
        if ((u >= 0) && (v >= 0) && (u + v < 1))
        {
            return true;
        }
        return false;
    }

    private bool CheckConvexFor(int index, PolyVertexInformation[] verts)
    {
        if (index == -1)
        {
            index = verts.Length - 1;
        }
        else if (index == verts.Length)
        {
            index = 0;
        }

        if (index == 0)
        {
            return IsConvexVertex(verts[verts.Length - 1].pos, verts[index].pos, verts[index + 1].pos);
        }
        else if (index == verts.Length - 1)
        {
            return IsConvexVertex(verts[index-1].pos, verts[index].pos, verts[0].pos);
        }
        else
        {
            return IsConvexVertex(verts[index - 1].pos, verts[index].pos, verts[index+1].pos);
        }
    }

    private bool IsConvexVertex(Vector3 p0, Vector3 p1, Vector3 p2)
    {
        float res = Vector3.SignedAngle(p1 - p0, p1 - p2, -Vector3.up);
        return res >= 0 && res <= 180;
    }

    private int GetIndexFromId(int id, PolyVertexInformation[] verts)
    {
        for (int i = 0; i < verts.Length; i++)
        {
            if(verts[i].id == id)
            {
                return i;
            }
        }
        return -1;
    }
}
