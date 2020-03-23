﻿using System;
using System.Collections;
using System.Collections.Generic;
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
        public Vector3 awayFromDoorDirection;
        public Transform entranceMidPoint;
        public float offset;
        [HideInInspector] public NavMeshGenerator generator;

        public void CalculateDoorDirection()
        {
            if(entrance1 != null && entrance2 != null && entranceMidPoint != null)
            {
                Vector3 entranceDirection = (entrance1.Position - entrance2.Position);
                awayFromDoorDirection = Vector3.Cross(entranceDirection, Vector3.up).normalized;
                entranceMidPoint.position = entrance2.Position + (entranceDirection) / 2 + offset * awayFromDoorDirection;
            }
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

        [SerializeField] private NavMeshEntrance[] entrancePoints = null;

        public void UpdateInformation()
        {
            RenameVertexes();
            UpdateAdjacentVertexesAndTriangleID();
            UpdateEntranceDoorDirections();
        }

        public NavMeshEntrance GetRanomEntrance
        {
            get { return entrancePoints[Random.Range(0, entrancePoints.Length)]; }
        }

        public NavMeshEntrance GetEntrance(int index)
        {
            return entrancePoints[index];
        }

        private void OnValidate()
        {
            if (!Application.isPlaying)
            {
                RenameVertexes();
                UpdateAdjacentVertexesAndTriangleID(0);
                UpdateEntranceDoorDirections();
            }
        }

        private void Awake()
        {
            UpdateAdjacentVertexesAndTriangleID();
            UpdateEntranceDoorDirections();
        }

        public void UpdateEntranceDoorDirections()
        {
            foreach (NavMeshEntrance entrance in entrancePoints)
            {
                entrance.generator = this;
                entrance.CalculateDoorDirection();
            }
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
            triangles = newTriangles;
            return newTriangles[newTriangles.Length - 1];
        }

        internal void CreateTriangle(Vertex vert1, Vertex vert2, Vertex vert3)
        {
            NavMeshTriangle tri = AddNewTriangle();
            tri.vertex1 = vert1;
            tri.vertex2 = vert2;
            tri.vertex3 = vert3;
            AddTriangleVertexesToAdjacencyList(tri);
        }

        private static void AddVertexesToAdjacentList(Vertex vert1, Vertex vert2)
        {
            if (vert1 != null && vert2 != null && !vert1.Adjacent.Contains(vert2))
            {
                vert1.AddAdjacentNode(vert2);
            }
        }

        private void OnDrawGizmos()
        {
            foreach (NavMeshTriangle tri in triangles)
            {
                tri.GizmoDrawTriangle(Color.green);
            }
            foreach (NavMeshEntrance item in entrancePoints)
            {
                if (item.entrance1 != null && item.entrance2 != null && item.entranceMidPoint != null)
                {
                    Gizmos.color = Color.yellow;
                    Gizmos.DrawSphere(item.entrance1.Position, 0.3f);
                    Gizmos.DrawLine(item.entrance1.Position, item.entrance2.Position);
                    Gizmos.DrawSphere(item.entrance2.Position, 0.3f);
                    Gizmos.DrawRay(item.entranceMidPoint.position, item.awayFromDoorDirection);
                    Handles.Label(item.entranceMidPoint.position + Vector3.left*0.5f, item.ID.ToString());
                }
            }
        }

        public NavMeshEntrance[] Entrances
        {
            get { return entrancePoints; }
        }
    }
}
