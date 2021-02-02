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

        public override int GetHashCode()
        {
            int hashCode = 780059485;
            hashCode = hashCode * -1521134295 + base.GetHashCode(); 
            hashCode = hashCode * -1521134295 + name.GetHashCode();
            hashCode = hashCode * -1521134295 + vertex.ID.GetHashCode();
            return hashCode;
        }
    }
    public class Vertex : MonoBehaviour
    {
        public void SetName(string name)
        {
            this.name = name;
        }

        public int ID = -1;

        [SerializeField] private DoorController doorController = null;
        public bool IsDoorway => doorController != null && doorController.IsDoorActive;
        public bool IsPassable => (doorController == null) || (doorController != null && doorController.IsPassable);
        public Vector3 Position => this.transform.position;
        public Vector3 savedPosition = new Vector3();
        public Vector3 LocalPosition => this.transform.localPosition;
        [SerializeField] private List<Vertex> adjacent;
        public List<Vertex> Adjacent => adjacent;
        private List<AdjacentVertex> adjacentInformation = new List<AdjacentVertex>();
        public List<AdjacentVertex> AdjacentInformation => adjacentInformation;
        public RoomInformation containedRoom = null;

        public int Count
        {
            get { return adjacent.Count; }
        }

        public DoorController GetDoorController => doorController;

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
            if (adjacentInformation == null)
            {
                adjacentInformation = new List<AdjacentVertex>();
            }
        }

        private void Start()
        {
            savedPosition = this.transform.position;
        }

        public void UpdateSavedPosition()
        {
            savedPosition = this.transform.position;
        }

        public override bool Equals(object other)
        {
            Vertex vert = (Vertex)other;
            return vert.savedPosition == savedPosition && vert.ID == ID;
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
            if(adjacent == null)
            {
                adjacent = new List<Vertex>();
            }
            if(adjacentInformation == null)
            {
                adjacentInformation = new List<AdjacentVertex>();
            }
            if (!adjacent.Contains(adjacentVertex))
            {
                adjacent.Add(adjacentVertex);
                adjacentInformation.Add(new AdjacentVertex(){vertex = adjacentVertex});
            }
        }

        public void SetDoorController(DoorController doorCont)
        {
            doorController = doorCont;
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

        public void RemoveAdjacentVertex(Vertex vertex)
        {
            adjacent.Remove(vertex);
        }
    }
}
