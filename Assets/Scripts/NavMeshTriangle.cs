using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Pieter.NavMesh
{
    [System.Serializable]
    public class NavMeshTriangle 
    {
        public Vertex vertex1;
        public Vertex vertex2;
        public Vertex vertex3;
        [HideInInspector] public int ID;
        public bool isSelected;
        public List<int> adjacentTriangles;
        /// <summary>
        /// Is this triangle connected to an entrance Vertex
        /// </summary>
        public bool isConnectedToEntrance = false;
        /// <summary>
        /// Which Entrances for the contained room
        /// </summary>
        public int[] connectedEntranceIDs = new int[0];

        public Vector3 centerPoint { get { return (vertex1.Position + vertex2.Position + vertex3.Position) / 3; } }

        public void GizmoDrawTriangle(Color color)
        {
            Gizmos.color = Color.red;
            if (Vector3.Dot(CalculateTriangleNormal(), Vector3.up) < 0.4f)
            {
                Gizmos.color = Color.yellow;
            }
            if (isSelected)
            {
                Gizmos.color = color;
            }
            if(vertex1 != null && vertex2 != null)
            {
                Gizmos.DrawLine(vertex1.Position, vertex2.Position);
            }
            if (vertex1 != null && vertex3 != null)
            {
                Gizmos.DrawLine(vertex1.Position, vertex3.Position);
            }
            if (vertex3 != null && vertex2 != null)
            {
                Gizmos.DrawLine(vertex3.Position, vertex2.Position);
            }
            if (vertex1 != null && vertex2 != null && vertex3 != null)
            {
                Gizmos.DrawRay((vertex1.Position + vertex2.Position + vertex3.Position) / 3, CalculateTriangleNormal().normalized / 3);
            }
        }

        public bool IsAdjacent(NavMeshTriangle tri)
        {
            // Check Vertex 1
            bool vert1 = tri.vertex1.Equals(vertex1) || tri.vertex2.Equals(vertex1) || tri.vertex3.Equals(vertex1);
            bool vert2 = tri.vertex1.Equals(vertex2) || tri.vertex2.Equals(vertex2) || tri.vertex3.Equals(vertex2);
            bool vert3 = tri.vertex1.Equals(vertex3) || tri.vertex2.Equals(vertex3) || tri.vertex3.Equals(vertex3);
            return vert1 || vert2 || vert3;
        }

        public Vector3 CalculateTriangleNormal()
        {
            if(vertex1.Position == null || vertex2.Position == null || vertex3.Position == null)
            {
                return Vector3.zero;
            }
            Vector3 U = vertex2.Position - vertex1.Position;
            Vector3 V = vertex3.Position - vertex1.Position;

            return Vector3.Cross(U,V);
        }

        public void AddConnectedEntrance(int ID)
        {
            int[] newIds = new int[connectedEntranceIDs.Length + 1];
            for (int i = 0; i < connectedEntranceIDs.Length; i++)
            {
                newIds[i] = connectedEntranceIDs[i];
            }
            newIds[connectedEntranceIDs.Length] = ID;
            connectedEntranceIDs = newIds;
        }

        // override object.Equals
        public override bool Equals(object obj)
        {
            //       
            // See the full list of guidelines at
            //   http://go.microsoft.com/fwlink/?LinkID=85237  
            // and also the guidance for operator== at
            //   http://go.microsoft.com/fwlink/?LinkId=85238
            //

            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }
            NavMeshTriangle tri = obj as NavMeshTriangle;
            if(tri == null)
            {
                return false;
            }
            return (tri.vertex1 != null && tri.vertex1.Equals(this.vertex1)) &&
                    (tri.vertex2 != null && tri.vertex2.Equals(this.vertex2)) &&
                    (tri.vertex3 != null && tri.vertex3.Equals(this.vertex3));
        }

        // override object.GetHashCode
        public override int GetHashCode()
        {
            // TODO: write your implementation of GetHashCode() here
            return base.GetHashCode();
        }
    }
}
