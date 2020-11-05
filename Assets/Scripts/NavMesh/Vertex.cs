using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace Pieter.NavMesh
{
    [System.Serializable]
    public struct AdjacentVertex 
    {
        [HideInInspector] public string name;
        public Vertex vertex;
        public float distanceToNode;

        public override bool Equals(object obj)
        {
            return vertex.Equals(obj);
        }
    }
    public class Vertex : MonoBehaviour
    {
        public void SetName(string name)
        {
            this.name = name;
        }

        public int ID = -1;
        private Vector3 lastSavedPosition = Vector3.zero;
        private Vector3 lastSavedLocalPosition = Vector3.zero;
        private int transformUpdateCounter = 0;
        private DoorController doorController = null;
        public bool isPassable => (doorController == null) || (doorController != null && doorController.IsPassable);
        public Vector3 Position
        {
            get
            {
                if (transformUpdateCounter++ % 50 == 0)
                {
                    transformUpdateCounter = 0;
                    try
                    {
                        lastSavedPosition = this.transform.position;
                    }
                    catch
                    {

                    }
                }
                return lastSavedPosition;
            }
        }

        public Vector3 LocalPosition
        {
            get
            {
                if (transformUpdateCounter++ % 50 == 0)
                {
                    transformUpdateCounter = 0;
                    try
                    {
                        lastSavedPosition = this.transform.localPosition;
                    }
                    catch
                    {

                    }
                }
                return lastSavedPosition;
            }
        }
        [SerializeField] private List<Vertex> adjacent;
        public List<Vertex> Adjacent => adjacent;
        private List<AdjacentVertex> adjacentInformation;
        public List<AdjacentVertex> AdjacentInformation => adjacentInformation;

        public int Count
        {
            get { return adjacent.Count; }
        }

        public Vertex GetAdjacentVertex(int index)
        {
            return adjacent[index];
        }
        public AdjacentVertex GetAdjacentVertexInfo(int index)
        {
            return adjacentInformation[index];
        }
        private void Awake()
        {
            doorController = GetComponentInChildren<DoorController>();
            if (adjacentInformation == null)
            {
                adjacentInformation = new List<AdjacentVertex>();
            }
            lastSavedPosition = this.transform.position;
        }
        public override bool Equals(object other)
        {
            Vertex vert = (Vertex)other;
            return vert.Position == Position;
        }

        public override string ToString()
        {
            return ID + " : " +Position.ToString();
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public void AddAdjacentNode(AdjacentVertex adjacentVertex)
        {

            if (!adjacentInformation.Contains(adjacentVertex) && !adjacent.Contains(adjacentVertex.vertex))
            {
                adjacent.Add(adjacentVertex.vertex);
                adjacentInformation.Add((adjacentVertex));
            }
        }
        public void AddAdjacentNode(Vertex adjacentVertex)
        {

            if (!adjacent.Contains(adjacentVertex))
            {
                adjacent.Add(adjacentVertex);
                adjacentInformation.Add(new AdjacentVertex(){vertex = adjacentVertex});
            }
        }

        public void UpdateAdjacentNode(int index, string name = "", Vertex vert = null, float distance = 0)
        {
            adjacent[index] = vert;
            adjacentInformation[index] = new AdjacentVertex {distanceToNode = distance, vertex = vert, name = name};
            
        }

        public void ResetAdjacentLists()
        {
            adjacent = new List<Vertex>();
            adjacentInformation = new List<AdjacentVertex>();
        }
    }
}
