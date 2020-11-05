using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

namespace Pieter.NavMesh
{
    [System.Serializable]
    public class NavMeshEntrance
    {
        public int ID = 0;
        public Vertex entrance1;
        public Vertex entrance2;
        public NavMeshTriangle connectedTriangle;
        public Vector3 awayFromDoorDirection;
        public Vertex entranceMidPoint;
        public float offset;
        public float spawnedRotation = 0;
        [HideInInspector] public NavMeshGenerator generator;
        public NavMeshEntrance connectedEntrance = null;
        public bool IsPassable => IsOpen && IsConnectedOpen;
        [SerializeField] private DoorController doorController = null;

        public bool IsConnectedOpen
        {
            get
            {
                return connectedEntrance.IsOpen;
            }
        }
        public bool IsOpen 
        { 
            get 
            {
                if (doorController)
                {
                    return doorController.IsPassable;
                }
                else
                {
                    return true;
                }
            } 
        }

        public void SetDoorController()
        {
            doorController = entranceMidPoint.GetComponentInChildren<DoorController>();
        }

        public void CalculateDoorDirection()
        {
            if(entrance1 != null && entrance2 != null && entranceMidPoint != null)
            {
                Vector3 entranceDirection = (entrance1.Position - entrance2.Position);
                awayFromDoorDirection = Vector3.Cross(entranceDirection, Vector3.up).normalized;
                entranceMidPoint.transform.position = entrance2.Position + (entranceDirection) / 2 + offset * awayFromDoorDirection;
                entranceMidPoint.transform.rotation = Quaternion.LookRotation(awayFromDoorDirection, Vector3.up);
            }
        }

        public Vector3 GetRotatedAwayFromDoorDirection()
        {
            return Quaternion.AngleAxis(spawnedRotation, Vector3.up) * awayFromDoorDirection;
        }
    }

    public class NavMeshGenerator : MonoBehaviour
    {
        public RoomInformation containedRoom;
        [SerializeField] private NavMeshTriangle[] triangles = null;


        private Vertex[] cachedVertex = null;
        public int GetNumberOfVertexes => transform.childCount;
        /// <summary>
        /// This is a very comp heavy function, use with caution
        /// </summary>
        public Vertex[] Vertexes
        {
            get
            {
                if (cachedVertex == null || cachedVertex.Length != GetNumberOfVertexes)
                {
                    cachedVertex = new Vertex[GetNumberOfVertexes];
                    for (int i = 0; i < cachedVertex.Length; i++)
                    {
                        cachedVertex[i] = transform.GetChild(i).GetComponent<Vertex>();
                    }
                }
                return cachedVertex;
            }
        }

        public NavMeshTriangle[] Triangles { get { return triangles; } }

        [SerializeField] private EntrancePoints entrancePoints = null;
        public void UpdateInformation()
        {
            RenameVertexes();
            UpdateAdjacentVertexesAndTriangleID();
            UpdateTriangleIsConnectedToEntrance();
        }

        private void UpdateTriangleIsConnectedToEntrance()
        {
            for (int i = 0; i < triangles.Length; i++)
            {
                for (int j = 0; j < entrancePoints.Length; j++)
                {
                    bool res = triangles[i].vertex1.Equals(entrancePoints.Entrances[j].entrance1) ||
                                triangles[i].vertex1.Equals(entrancePoints.Entrances[j].entrance2) ||
                                triangles[i].vertex2.Equals(entrancePoints.Entrances[j].entrance1) ||
                                triangles[i].vertex2.Equals(entrancePoints.Entrances[j].entrance2) ||
                                triangles[i].vertex3.Equals(entrancePoints.Entrances[j].entrance1) ||
                                triangles[i].vertex3.Equals(entrancePoints.Entrances[j].entrance2);
                    if (res)
                    {
                        triangles[i].isConnectedToEntrance = true;
                        triangles[i].AddConnectedEntrance(entrancePoints.Entrances[j].ID);
                        Debug.Log("Triangle " + i + " is an entrance Triangle");
                    }
                }
            }
        }

        public NavMeshEntrance GetRandomGenerator
        {
            get { return entrancePoints.GetEntrance(Random.Range(0, entrancePoints.Length)); }
        }

        public NavMeshEntrance GetEntrance(int index)
        {
            return entrancePoints.GetEntrance(index);
        }

        private void Awake()
        {
            UpdateAdjacentVertexesAndTriangleID();
        }


        public int UpdateAdjacentVertexesAndTriangleID(int counterDefault = 0)
        {
            int counter = counterDefault;
            foreach (NavMeshTriangle triangle in triangles)
            {
                triangle.ID = counter++; 
                AddTriangleVertexesToAdjacencyList(triangle);
            }
            return counter;
        }


        private void UpdatingTrisThread(object obj)
        {
            AdjacentTriangles adjacentTriangles = (AdjacentTriangles)obj;
            Debug.Log(adjacentTriangles.from + " " + adjacentTriangles.to + " Start");
            for (int i = adjacentTriangles.from; i < adjacentTriangles.to; i++)
            {
                AddAdjacentTriangles(triangles[i]);
            }
            Debug.Log("Done " + adjacentTriangles.from + " to " + adjacentTriangles.to);
        }

        public void UpdateAdjacentTriangles()
        {
            foreach (NavMeshTriangle triangle in triangles)
            {
                triangle.adjacentTriangles.Clear();
            }
            Thread t = new Thread(new ParameterizedThreadStart(UpdatingTrisThread));
            Thread t2 = new Thread(new ParameterizedThreadStart(UpdatingTrisThread));
            t.Start(new AdjacentTriangles() { from = 0, to = triangles.Length / 2 });
            t2.Start(new AdjacentTriangles() { from = triangles.Length / 2, to = triangles.Length });
        }

        private static void AddTriangleVertexesToAdjacencyList(NavMeshTriangle triangle)
        {
            AddVertexesToAdjacentList(triangle.vertex1, triangle.vertex2);
            AddVertexesToAdjacentList(triangle.vertex1, triangle.vertex3);

            AddVertexesToAdjacentList(triangle.vertex2, triangle.vertex1);
            AddVertexesToAdjacentList(triangle.vertex2, triangle.vertex3);

            AddVertexesToAdjacentList(triangle.vertex3, triangle.vertex1);
            AddVertexesToAdjacentList(triangle.vertex3, triangle.vertex2);
        }

        public int RenameVertexes(int counterDefault = 0)
        {
            int counter = counterDefault;
            foreach (Transform item in this.transform)
            {
                item.name = "Vertex" + counter++;
                Vertex vert = item.GetComponent<Vertex>();
                if(vert == null)
                {
                    vert = item.gameObject.AddComponent<Vertex>();
                }
                vert.ID = counter - 1;
                vert.ResetAdjacentLists();
            }
            return counter;
        }

        public NavMeshTriangle AddNewTriangle()
        {
            NavMeshTriangle[] newTriangles = new NavMeshTriangle[triangles.Length + 1];
            for (int i = 0; i < triangles.Length; i++)
            {
                newTriangles[i] = triangles[i];
            }
            newTriangles[newTriangles.Length - 1] = new NavMeshTriangle();
            newTriangles[newTriangles.Length - 1].adjacentTriangles = new List<int>();
            triangles = newTriangles;
            return newTriangles[newTriangles.Length - 1];
        }

        public void CreateTriangle(Vertex vert1, Vertex vert2, Vertex vert3)
        {
            NavMeshTriangle tri = AddNewTriangle();
            tri.vertex1 = vert1;
            tri.vertex2 = vert2;
            tri.vertex3 = vert3;
            AddTriangleVertexesToAdjacencyList(tri);
            AddAdjacentTriangles(tri);
        }

        private void AddAdjacentTriangles(NavMeshTriangle tri)
        {
            tri.adjacentTriangles.Clear();
            // Check if a duo of verts share more than 1 adjacent vertex
            NavMeshTriangle triangle = GetAdjacentTriangle(tri.vertex1, tri.vertex2, tri.vertex3);
            if (triangle != null)
            {
                tri.adjacentTriangles.Add(triangle.ID);
            }

            triangle = GetAdjacentTriangle(tri.vertex2, tri.vertex3, tri.vertex1);
            if (triangle != null)
            {
                tri.adjacentTriangles.Add(triangle.ID);
            }

            triangle = GetAdjacentTriangle(tri.vertex3, tri.vertex1, tri.vertex2);
            if (triangle != null)
            {
                tri.adjacentTriangles.Add(triangle.ID);
            }

        }

        private NavMeshTriangle GetAdjacentTriangle(Vertex vert1, Vertex vert2, Vertex vert3)
        {
            Vertex sharedVert = null;
            foreach (Vertex vertex in vert1.Adjacent)
            {
                foreach (Vertex vert in vert2.Adjacent)
                {
                    if (vertex.Equals(vert) && !vert3.Equals(vert))
                    {
                        sharedVert = vert;
                    }
                }
            }

            if (sharedVert != null)
            {
                return FindTriangle(vert1, vert2, sharedVert);
            }

            return null;
        }

        public NavMeshTriangle FindTriangle(Vertex vert1, Vertex vert2, Vertex sharedVert)
        {
            foreach (NavMeshTriangle triangle in triangles)
            {
                if ((triangle.vertex1.Equals(vert1) || triangle.vertex1.Equals(vert2) || triangle.vertex1.Equals(sharedVert)) &&
                    (triangle.vertex2.Equals(vert1) || triangle.vertex2.Equals(vert2) || triangle.vertex2.Equals(sharedVert)) &&
                    (triangle.vertex3.Equals(vert1) || triangle.vertex3.Equals(vert2) || triangle.vertex3.Equals(sharedVert)))
                {
                    return triangle;
                }
            }

            return null;
        }

        private static void AddVertexesToAdjacentList(Vertex vert1, Vertex vert2)
        {
            if (vert1 != null && vert2 != null && !vert1.Adjacent.Contains(vert2))
            {
                vert1.AddAdjacentNode(vert2);
            }
        }

        public NavMeshTriangle FindTriangleWithID(int id)
        {
            for (int i = 0; i < triangles.Length; i++)
            {
                if(triangles[i].ID == id)
                {
                    return triangles[i];
                }
            }
            return null;
        }

        private void OnDrawGizmos()
        {
            foreach (NavMeshTriangle tri in triangles)
            {
                tri.GizmoDrawTriangle(Color.green);
                for (int i = 0; i < tri.adjacentTriangles.Count; i++)
                {
                    Gizmos.color = Color.cyan;

                    Gizmos.DrawLine(tri.centerPoint, FindTriangleWithID(tri.adjacentTriangles[i]).centerPoint);
                }
            }

            if (entrancePoints != null)
            {
                foreach (NavMeshEntrance item in entrancePoints.Entrances)
                {
                    if (item.entrance1 != null && item.entrance2 != null && item.entranceMidPoint != null)
                    {
                        Gizmos.color = Color.yellow;
                        Gizmos.DrawSphere(item.entrance1.Position, 0.3f);
                        Gizmos.DrawLine(item.entrance1.Position, item.entrance2.Position);
                        Gizmos.DrawSphere(item.entrance2.Position, 0.3f);
                        Gizmos.DrawRay(item.entranceMidPoint.transform.position, item.awayFromDoorDirection);
                        Handles.Label(item.entranceMidPoint.transform.position + Vector3.left * 0.5f, item.ID.ToString());
                    }
                }
            }
        }


        public NavMeshEntrance[] Entrances
        {
            get { return entrancePoints.Entrances; }
        }
    }
}
