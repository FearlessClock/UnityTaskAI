using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

namespace Pieter.NavMesh
{
    public class NavMeshHolder : MonoBehaviour
    {
        [SerializeField] private NavMeshTriangle[] triangles = new NavMeshTriangle[0];
        [SerializeField] private Vertex[] vertexes = new Vertex[0];

        public NavMeshTriangle[] Triangles { get { return triangles; } }

        public Vertex[] Vertexes { get { return vertexes; } }

        private Action OnValueUpdated;
        public void AddListener(Action call)
        {
            OnValueUpdated += call;
        }
        public void RemoveListener(Action call)
        {
            OnValueUpdated -= call;
        }
        public void Notify()
        {
            OnValueUpdated?.Invoke();
        }

        public void CollectTriangles(NavMeshGenerator[] generators)
        {
            List<NavMeshTriangle> collectedTriangels = new List<NavMeshTriangle>();
            foreach (NavMeshGenerator navMesh in generators)
            {
                collectedTriangels.AddRange(navMesh.Triangles);
            }
            triangles = collectedTriangels.ToArray();
            int counter = 0;
            for (int i = 0; i < triangles.Length; i++)
            {
                //triangles[i].ID = counter++;
            }
        }

        public void AddTriangles(NavMeshGenerator generators)
        {
            List<NavMeshTriangle> collectedTriangles = new List<NavMeshTriangle>();
            collectedTriangles.AddRange(triangles);
            collectedTriangles.AddRange(generators.Triangles);
            triangles = collectedTriangles.ToArray();
            int counter = 0;
            for (int i = 0; i < triangles.Length; i++)
            {
                //triangles[i].ID = counter++;
            }
        }

        public void AddVertexes(NavMeshGenerator generators)
        {
            List<Vertex> collectedVertexes = new List<Vertex>();
            collectedVertexes.AddRange(vertexes);
            collectedVertexes.AddRange(generators.Vertexes);
            vertexes = collectedVertexes.ToArray();
            int counter = 0;
            for(int i = 0; i < vertexes.Length; i++)
            {
                //vertexes[i].ID = counter++;
            }
        }

        public NavMeshTriangle GetContainingTriangle(Vector3 pos)
        {
            for(int i = 0; i < triangles.Length; i++)
            {
                if (IsPositionInTriangle(pos, triangles[i]))
                {
                    return triangles[i];
                }
            }
            return null;
        }

        private bool IsPositionInTriangle(Vector3 pos, NavMeshTriangle triangle)
        {
            // Compute vectors        
            Vector3 v0 = triangle.vertex2.Position - triangle.vertex1.Position;
            Vector3 v1 = triangle.vertex3.Position - triangle.vertex1.Position;
            Vector3 v2 = pos - triangle.vertex1.Position;

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

        public NavMeshTriangle GetRandomTriangle()
        {
            if (triangles.Length == 0)
            {
                return null;
            }
            int index = Random.Range(0, triangles.Length);

            return triangles[index];
        }

        public Vector3 GetRandomPointInTriangle(NavMeshTriangle triangle)
        {
            float x = Random.value;
            float y = Random.value;
            while (x + y > 1)
            {
                x = Random.value;
                y = Random.value;
            }
            Vector3 insideTriangle = (triangle.vertex2.Position - triangle.vertex1.Position) * x + (triangle.vertex3.Position - triangle.vertex1.Position) * y;
            return triangle.vertex1.Position + insideTriangle;
        }

        private void OnDrawGizmos()
        {
        //    foreach (NavMeshTriangle tri in triangles)
        //    {
        //        tri.GizmoDrawTriangle(Color.green);
        //    }
        }

        public void AddNavMesh(NavMeshGenerator meshGenerator)
        {
            AddTriangles(meshGenerator);
            AddVertexes(meshGenerator);
            Notify();
        }

        public void ResetData()
        {
            triangles = new NavMeshTriangle[0];
            vertexes = new Vertex[0];
        }
    }
}
