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

        public void GizmoDrawTriangle(Color color)
        {
            Gizmos.color = isSelected? color: Color.red;
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

        public Vector3 CalculateTriangleNormal()
        {
            Vector3 U = vertex2.Position - vertex1.Position;
            Vector3 V = vertex3.Position - vertex1.Position;

            return Vector3.Cross(U,V);
        }
    }
}
