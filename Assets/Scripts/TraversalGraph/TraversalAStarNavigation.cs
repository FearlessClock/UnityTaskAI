using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pieter.NavMesh;
using UnityEngine;

namespace Pieter.GraphTraversal
{
    public class TraversalAStarNavigation
    {
        private TraversalGraphHolder traversalGraph;
        // GetPath Variables
        List<NavMeshMovementLine> path = new List<NavMeshMovementLine>();
        List<AStarPoint> open = new List<AStarPoint>();
        AStarPoint current;

        AStarPoint[] actualPoints;
        public TraversalAStarNavigation(TraversalGraphHolder traversalGraph)
        {
            this.traversalGraph = traversalGraph;
            this.traversalGraph.AddListener(UpdateVertexes);
            UpdateVertexes();
        }

        private void UpdateVertexes()
        {
            actualPoints = new AStarPoint[traversalGraph.TraversalLines.Length];
            for (int i = 0; i < traversalGraph.TraversalLines.Length; i++)
            {
                actualPoints[i] = CreateAStarPoint(0, 0, traversalGraph.TraversalLines[i].vertex, Vector3.zero);
            }
        }

        private AStarPoint GetPointValue(Vertex chosen)
        {
            for (int i = 0; i < actualPoints.Length; i++)
            {
                if (actualPoints[i].vert == chosen)
                {
                    return actualPoints[i];
                }
            }
            return null;
        }

        public List<NavMeshMovementLine> GetPathFromTo(Vertex from, Vertex to)
        {
            if (actualPoints.Length == 0)
            {
                return new List<NavMeshMovementLine>();
            }
            for (int i = 0; i < actualPoints.Length; i++)
            {
                actualPoints[i].inClosed = false;
                actualPoints[i].f = 0;
                actualPoints[i].g = 0;
                actualPoints[i].h = 0;
                actualPoints[i].parent = null;
            }

            if (from == null)
            {
                return new List<NavMeshMovementLine>();
            }

            if (to == null)
            {
                return new List<NavMeshMovementLine>();
            }
            path.Clear();

            // If the points are on the same triangle, send back a straight line between both points
            if (from == to)
            {
                path.Add(new NavMeshMovementLine { point = from.Position });
                path.Add(new NavMeshMovementLine { point = to.Position });
                return path;
            }

            open.Clear();
            // Add the starting triangle vertexes to the open list
            open.Add(UpdateAStarPoint(GetPointValue(from), 0, 0, to.Position));

            // The starting point does not need to be kept in the closed list

            while (open.Count > 0)
            {
                int index = 0;
                float lowest = open[0].f;
                for (int i = 1; i < open.Count; i++)
                {
                    if (open[i].f < lowest)
                    {
                        lowest = open[i].f;
                        index = i;
                    }
                }

                current = open[index];

                if (current.vert.Equals(to))
                {
                    return ReconstructPath(current, to.Position, from.Position);
                }

                open.RemoveAt(index);
                current.inClosed = true;
                if (open.Count > 9999)
                {
                    break;
                }
                for (int i = 0; i < current.vert.Count; i++)
                {
                    if (!current.vert.GetAdjacentVertex(i).isPassable)
                    {
                        continue;
                    }
                    AStarPoint aStarPointExisting = GetPointValue(current.vert.GetAdjacentVertex(i));
                    if (aStarPointExisting.inClosed)
                    {
                        continue;
                    }

                    float cost = current.g + (Mathf.Pow((current.vert.Position.x - current.vert.GetAdjacentVertex(i).Position.x), 2) + Mathf.Pow((current.vert.Position.y - current.vert.GetAdjacentVertex(i).Position.y), 2));
                    AStarPoint aStarPoint = open.Find((item) => item.vert.Equals(current.vert.GetAdjacentVertex(i)));
                    if (aStarPoint == null)
                    {
                        aStarPoint = UpdateAStarPoint(aStarPointExisting, current.g, (Mathf.Pow((current.vert.Position.x - current.vert.GetAdjacentVertex(i).Position.x), 2) + Mathf.Pow((current.vert.Position.y - current.vert.GetAdjacentVertex(i).Position.y), 2)), to.Position, current);

                        open.Add(aStarPoint);
                    }
                    else
                    {
                        if (cost < aStarPoint.g)
                        {
                            aStarPoint.g = cost;
                            aStarPoint.f = aStarPoint.g + aStarPoint.h;
                            aStarPoint.parent = current;
                        }
                    }
                }
            }

            return path;
        }

        private List<NavMeshMovementLine> reconstructedPath = new List<NavMeshMovementLine>();
        private List<NavMeshMovementLine> ReconstructPath(AStarPoint lastPoint, Vector3 end, Vector3 start)
        {
            reconstructedPath.Clear();
            reconstructedPath.Add(new NavMeshMovementLine { point = lastPoint.vert.Position, associatedVertex = lastPoint.vert });
            current = lastPoint.parent;
            while (current != null)
            {
                reconstructedPath.Add(new NavMeshMovementLine { point = current.vert.Position, associatedVertex = lastPoint.vert });
                lastPoint = current;
                current = lastPoint.parent;
            }
            reconstructedPath.Reverse();
            return reconstructedPath;
        }

        private static AStarPoint CreateAStarPoint(float oldG, float distance, Vertex current, Vector3 end, AStarPoint parent = null)
        {
            float h = (Mathf.Pow((current.Position.x - end.x), 2) + Mathf.Pow((current.Position.y - end.y), 2));
            float g = oldG + distance;
            return new AStarPoint() { vert = current, g = g, h = h, f = h + g, parent = parent };
        }
        private static AStarPoint UpdateAStarPoint(AStarPoint value, float oldG, float distance, Vector3 end, AStarPoint parent = null)
        {
            if (value == null)
            {
                return value;
            }
            float h = (Mathf.Pow((value.vert.Position.x - end.x), 2) + Mathf.Pow((value.vert.Position.y - end.y), 2));
            float g = oldG + distance;
            value.g = g;
            value.h = h;
            value.f = h + g;
            value.parent = parent;
            return value;
        }
    }
}
