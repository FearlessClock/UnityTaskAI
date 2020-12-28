using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using Pieter.NavMesh;
using UnityEngine;
using UnityEngine.Assertions.Must;
using UnityEngine.Serialization;

namespace Pieter.GraphTraversal
{
    [System.Serializable]
    public class TraversalLine
    {
        [HideInInspector] public string name;
        public Vertex vertex;
        public List<Vertex> adjacentVertexes;
        [HideInInspector] public TraversalGenerator generator;
        [SerializeField] private bool isDoorway = false;
        public bool IsDoorway => isDoorway;
        public override string ToString()
        {
            return name +" " + vertex;
        }
    }
    [System.Serializable]
    public class TraversalEntrance
    {
        [HideInInspector] public string name;
        public int ID => vertex.ID;
        public Vertex vertex;
    }
    public class TraversalGenerator : MonoBehaviour
    {
        [SerializeField] private bool dontUpdate = false;
        public RoomInformation containedRoom;
        [SerializeField] private TraversalLine[] traversalLines = null;
        [SerializeField] private TraversalEntrance[] entrances = null;

        private Vector3 middleOfTheRoom;
        public Vector3 MiddleOfRoom => middleOfTheRoom;
        public TraversalLine[] TraversalLines => traversalLines;

        public Vertex ClosestVertex(Vector3 position)
        {
            float closest = Vector3.Distance(position, traversalLines[0].vertex.Position);
            int index = 0;
            for (int i = 0; i < traversalLines.Length; i++)
            {
                if(traversalLines[i].IsDoorway)
                {
                    continue;
                }
                if (closest > Vector3.Distance(position, traversalLines[i].vertex.Position))
                {
                    closest = Vector3.Distance(position, traversalLines[i].vertex.Position);
                    index = i;
                }
            }

            return traversalLines[index].vertex;
        }
        
        private void OnValidate()
        {
            if (!dontUpdate)
            {
                UpdateInfo();
            }
        }
        private void Start()
        {
            UpdateRoomMiddle();
        }

        public void UpdateInfo()
        {
            RenameVertexes();
            UpdateAdjacencyLists();
            UpdateRoomMiddle();
        }

        private void UpdateRoomMiddle()
        {
            middleOfTheRoom = new Vector3();
            foreach (TraversalLine line in traversalLines)
            {
                middleOfTheRoom += line.vertex.Position;
            }
            middleOfTheRoom /= traversalLines.Length;
        }

        private int RenameVertexes(int counterDefault = 0)
        {
            int counter = counterDefault;
            foreach (Transform item in this.transform)
            {
                item.name = this.transform.parent.name + "Vertex" + counter++;
                Vertex vert = item.GetComponent<Vertex>();
                if (vert == null)
                {
                    vert = item.gameObject.AddComponent<Vertex>();
                }
                vert.containedRoom = containedRoom;
                vert.ID = counter - 1;
                vert.ResetAdjacentLists();
            }
            return counter;
        }

        private void UpdateAdjacencyLists()
        {
            foreach (TraversalLine traversalLine in traversalLines)
            {
                if (traversalLine == null)
                {
                    continue;
                }

                traversalLine.generator = this;
                if (traversalLine.vertex != null)
                {
                    foreach (TraversalLine traversalLineOther in traversalLines)
                    {
                        if (traversalLine.vertex == traversalLineOther.vertex)
                        {
                            continue;
                        }

                        foreach (Vertex adjacent in traversalLineOther.adjacentVertexes)
                        {
                            if (traversalLine.vertex == adjacent && !traversalLine.adjacentVertexes.Contains(traversalLineOther.vertex))
                            {
                                traversalLine.adjacentVertexes.Add(traversalLineOther.vertex);
                                Debug.Log(traversalLineOther.vertex.name);
                            }
                        }

                    }
                    traversalLine.name = traversalLine.vertex.name ;
                }

                // Move all the adjacent vertexes to the vertex adjacency list
                traversalLine.vertex.ResetAdjacentLists();
                for (int i = 0; i < traversalLine.adjacentVertexes.Count; i++)
                {
                    traversalLine.vertex.AddAdjacentNode(traversalLine.adjacentVertexes[i]);
                }

            }
        }

        public void AddAdjacentNodes(Vertex a, Vertex b)
        {
            for (int i = 0; i < traversalLines.Length; i++)
            {
                if (traversalLines[i].vertex == a)
                {
                    if (!traversalLines[i].adjacentVertexes.Contains(b))
                    {
                        traversalLines[i].adjacentVertexes.Add(b);
                    }
                }
            }
            a.AddAdjacentNode(b);
        }

        public void FuseNode(Vertex vertex1, Vertex vertex2, TraversalGenerator neighborGenerator, EntrancePoints entrancePoints)
        {
            UpdateGeneratorWithFusedNode(this, vertex1, vertex2);

            for (int i = 0; i < neighborGenerator.traversalLines.Length; i++)
            {
                if (neighborGenerator.traversalLines[i].vertex == vertex2)
                {
                    List<TraversalLine> updated = new List<TraversalLine>();
                    updated.AddRange(neighborGenerator.traversalLines);
                    updated.RemoveAt(i);
                    neighborGenerator.traversalLines = updated.ToArray();
                }
                else
                {
                    for (int j = 0; j < vertex2.Adjacent.Count; j++)
                    {
                        if (neighborGenerator.traversalLines[i].vertex == vertex2.Adjacent[j])
                        {
                            neighborGenerator.traversalLines[i].adjacentVertexes.Remove(vertex2);
                        }
                    }
                }
            }
            List<TraversalEntrance> updatedEntrances = new List<TraversalEntrance>();
            for (int i = 0; i < neighborGenerator.entrances.Length; i++)
            {
                if(neighborGenerator.entrances[i].vertex.ID != vertex2.ID)
                {
                    updatedEntrances.Add(neighborGenerator.entrances[i]);
                }
            }
            neighborGenerator.entrances = updatedEntrances.ToArray();
            entrancePoints.SwapVertexWithAnother(vertex2, vertex1);
            neighborGenerator.AddEntranceNode(vertex1);
            for (int i = 0; i < vertex2.Adjacent.Count; i++)
            {
                vertex1.AddAdjacentNode(vertex2.Adjacent[i]);
            }
            vertex2.gameObject.SetActive(false);
        }

        private void UpdateGeneratorWithFusedNode(TraversalGenerator generator, Vertex vertex1, Vertex vertex2)
        {
            for (int i = 0; i < generator.traversalLines.Length; i++)
            {
                if (generator.traversalLines[i].vertex == vertex1)
                {
                    for (int j = 0; j < vertex2.Adjacent.Count; j++)
                    {
                        if (!generator.traversalLines[i].adjacentVertexes.Contains(vertex2.Adjacent[j]))
                        {
                            generator.traversalLines[i].adjacentVertexes.Add(vertex2.Adjacent[j]);
                            vertex2.Adjacent[j].RemoveAdjacentVertex(vertex2);
                            vertex2.Adjacent[j].AddAdjacentNode(vertex1);
                        }
                    }
                    break;
                }
            }
        }

        private void AddEntranceNode(Vertex vertex)
        {
            List<TraversalEntrance> updatedEntrance = new List<TraversalEntrance>();
            updatedEntrance.AddRange(entrances);
            updatedEntrance.Add(new TraversalEntrance() { vertex = vertex });
            entrances = updatedEntrance.ToArray();
        }

        public void RemoveNode(Vertex vertex)
        {
            List<TraversalLine> updated = new List<TraversalLine>(); 
            for (int i = 0; i < traversalLines.Length; i++)
            {
                if (!traversalLines[i].vertex.Equals(vertex))
                {
                    updated.Add(traversalLines[i]);
                }
                traversalLines[i].adjacentVertexes.Remove(vertex);
            }
            traversalLines = updated.ToArray();

            List<TraversalEntrance> updatedEntrance = new List<TraversalEntrance>();
            for (int i = 0; i < entrances.Length; i++)
            {
                if (!entrances[i].vertex.Equals(vertex))
                {
                    updatedEntrance.Add(entrances[i]);
                }
            }
            entrances = updatedEntrance.ToArray();
            for (int i = 0; i < vertex.Adjacent.Count; i++)
            {
                vertex.Adjacent[i].RemoveAdjacentVertex(vertex);
            }
            vertex.ResetAdjacentLists();
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.yellow;
            if (traversalLines != null && traversalLines.Length > 0)
            {
                for (int i = 0; i < traversalLines.Length; i++)
                {
                    for (int j = 0; j < traversalLines[i].adjacentVertexes.Count; j++)
                    {
                        if (traversalLines[i].vertex != null)
                        {
                            Gizmos.DrawLine(traversalLines[i].vertex.Position, traversalLines[i].adjacentVertexes[j].Position);
                        }
                    }
                }
            }
            Gizmos.color = Color.grey;
            Gizmos.DrawSphere(MiddleOfRoom, 0.4f);
        }


        public TraversalEntrance GetEntrance(int id)
        {
            for (int i = 0; i < entrances.Length; i++)
            {
                if (entrances[i].ID == id)
                {
                    return entrances[i];
                }
            }

            return null;
        }
    }
}
