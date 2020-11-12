using System.Collections.Generic;
using UnityEngine;

namespace Pieter.NavMesh
{
    public class AStarPoint
    {
        public bool inClosed = false;
        public Vertex vert;
        public float g;
        public float h;
        public float f;
        public AStarPoint parent;

        public override string ToString()
        {
            return vert + " : " + g + " "+ h + " " + f ;
        }
    }

    public class AStarNavMeshNavigation
    {
        private const int numberOfSteps = 13;
        private NavMeshHolder navMesh;
        // GetPath Variables
        private NavMeshTriangle triStart;
        private NavMeshTriangle triEnd;
        List<NavMeshMovementLine> path = new List<NavMeshMovementLine>();
        List<AStarPoint> open = new List<AStarPoint>();
        AStarPoint current;
        AStarPoint outValue;

        AStarPoint[] actualPoints;
        public AStarNavMeshNavigation(NavMeshHolder navMesh)
        {
            this.navMesh = navMesh;
            this.navMesh.AddListener(UpdateVertexes);
            UpdateVertexes();
        }

        private void UpdateVertexes()
        {
            actualPoints = new AStarPoint[navMesh.Vertexes.Length];
            for (int i = 0; i < navMesh.Vertexes.Length; i++)
            {
                actualPoints[i] = CreateAStarPoint(0, 0, navMesh.Vertexes[i], Vector3.zero);
            }
        }

        private AStarPoint GetPointValue(int id)
        {
            return actualPoints[id];
        }

        public List<NavMeshMovementLine> GetPathFromTo(Vector3 from, Vector3 to, bool keepStartingNode = true, bool keepEndingNode= true)
        {
            if(actualPoints.Length == 0)
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
            triStart = navMesh.GetContainingTriangle(from);
            if(triStart == null)
            {
                return new List<NavMeshMovementLine>();
            }
            triEnd = navMesh.GetContainingTriangle(to);
            if (triEnd == null)
            {
                return new List<NavMeshMovementLine>();
            }
            path.Clear();

            // If the points are on the same triangle, send back a straight line between both points
            if (triStart.ID == triEnd.ID)
            {
                if (keepStartingNode)
                {
                    path.Add(new NavMeshMovementLine { point = from });
                }
                if (keepEndingNode)
                {
                    path.Add(new NavMeshMovementLine { point = to });
                }
                return path;
            }

            open.Clear();
            // Add the starting triangle vertexes to the open list
            open.Add(UpdateAStarPoint(GetPointValue(triStart.vertex1.ID), 0, 0, to));
            open.Add(UpdateAStarPoint(GetPointValue(triStart.vertex2.ID), 0, 0, to));
            open.Add(UpdateAStarPoint(GetPointValue(triStart.vertex3.ID), 0, 0, to));

            // The starting point does not need to be kept in the closed list

            while (open.Count > 0)
            {
                open.Sort((a, b) => a.f.CompareTo(b.f));
                current = open[0];

                if (current.vert.ID == (triEnd.vertex1.ID) ||
                   current.vert.ID == (triEnd.vertex2.ID) ||
                   current.vert.ID == (triEnd.vertex3.ID))
                {
                    return ReconstructPath(current, to, from, keepStartingNode, keepEndingNode);
                }

                open.RemoveAt(0);
                current.inClosed = true;
                if(open.Count > 4000)
                {
                    break;
                }
                for(int i = 0; i < current.vert.Count; i++)
                {
                    AStarPoint aStarPointExisting = GetPointValue(current.vert.GetAdjacentVertex(i).ID);
                    if (aStarPointExisting.inClosed)
                    {
                        continue;
                    }

                    float cost = current.g + (current.vert.Position - current.vert.GetAdjacentVertex(i).Position).magnitude;
                    AStarPoint aStarPoint = open.Find((item) => item.vert.Equals(current.vert.GetAdjacentVertex(i)));
                    if (aStarPoint == null)
                    {
                        aStarPoint = UpdateAStarPoint(aStarPointExisting, current.g, (Mathf.Pow((current.vert.Position.x - current.vert.GetAdjacentVertex(i).Position.x), 2)+ Mathf.Pow((current.vert.Position.y - current.vert.GetAdjacentVertex(i).Position.y), 2)), to, current);

                        open.Add(aStarPoint);
                    }
                    else 
                    {
                        if(cost < aStarPoint.g)
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
        private List<NavMeshMovementLine> ReconstructPath(AStarPoint lastPoint, Vector3 end, Vector3 start, bool keepStartingNode, bool keepEndingNode)
        {
            reconstructedPath = new List<NavMeshMovementLine>();
            if (keepEndingNode)
            {
                reconstructedPath.Add(new NavMeshMovementLine { point = end });
            }
            reconstructedPath.Add(new NavMeshMovementLine { point = lastPoint.vert.Position });
            current = lastPoint.parent;
            while(current != null)
            {
                reconstructedPath.Add(new NavMeshMovementLine { point = current.vert.Position });
                lastPoint = current;
                current = lastPoint.parent;
            }
            if (keepStartingNode)
            {
                reconstructedPath.Add(new NavMeshMovementLine { point = start });
            }
            reconstructedPath.Reverse();
            reconstructedPath = SmoothPath(reconstructedPath);
            return reconstructedPath;
        }

        private NavMeshMovementLine currentPoint;
        public List<NavMeshMovementLine> SmoothPath(List<NavMeshMovementLine> oldPath)
        {
            if (oldPath.Count <= 2)
            {
                return oldPath;
            }
            NavMeshTriangle triangle = null;
            for (int i = 0; i < oldPath.Count - 2; i++)
            {
                if (oldPath[i] == null)
                {
                    continue;
                }
                currentPoint = oldPath[i];
                for (int j = i + 2; j - (i + 2) <= 5 && j < oldPath.Count; j++)
                {
                    if (oldPath[j] == null)
                    {
                        continue;
                    }
                    bool canRemove = true;
                    Vector3 nextStep = Vector3.Lerp(currentPoint.point, oldPath[j].point, 0);
                    for (int k = 1; k < numberOfSteps; k++)
                    {
                        triangle = navMesh.GetContainingTriangle(nextStep);
                        if (triangle == null)
                        {
                            canRemove = false;
                            break;
                        }
                        nextStep = Vector3.Lerp(currentPoint.point, oldPath[j].point, k / (float)numberOfSteps);
                    }
                    if (canRemove)
                    {
                        oldPath[j - 1] = null;
                    }
                }
            }
            List<NavMeshMovementLine> newPath = new List<NavMeshMovementLine>();
            oldPath.ForEach((item) => { if (item != null) newPath.Add(item); });
            //List<NavMeshMovementLine> newPath = AvoidWalls(newPath);
            return newPath;
        }

        private static List<NavMeshMovementLine> AvoidWalls(List<NavMeshMovementLine> newPath)
        {
            for (int i = 1; i < newPath.Count - 1; i++)
            {
                Vector3 beforeDir = (newPath[i - 1].point - newPath[i].point).normalized;
                float beforeDirAngle = Vector3.SignedAngle(beforeDir, Vector3.forward, Vector3.up);
                Vector3 afterDir = (newPath[i + 1].point - newPath[i].point).normalized;
                float angle = Vector3.SignedAngle(afterDir, beforeDir, Vector3.up);
                //angle = (360 - angle) / 2;
                if (angle < 0)
                {
                    Vector3 normal = new Vector3(Mathf.Cos(angle * Mathf.Deg2Rad), 0, Mathf.Sin(angle * Mathf.Deg2Rad));
                    Debug.DrawRay(newPath[i].point, normal);
                    newPath[i].point += normal;
                }
                else
                {
                    Vector3 normal = new Vector3(Mathf.Cos(angle * Mathf.Deg2Rad), 0, Mathf.Sin(angle * Mathf.Deg2Rad));
                    Debug.DrawRay(newPath[i].point, normal);
                    newPath[i].point += normal;
                }
            }

            return newPath;
        }

        private static AStarPoint CreateAStarPoint(float oldG, float distance, Vertex current, Vector3 end, AStarPoint parent = null)
        {
            float h = (Mathf.Pow((current.Position.x - end.x), 2) + Mathf.Pow((current.Position.y - end.y), 2));
            float g = oldG + distance;
            return new AStarPoint() { vert = current, g = g, h = h, f = h+g, parent = parent };
        }
        private static AStarPoint UpdateAStarPoint(AStarPoint value, float oldG, float distance, Vector3 end, AStarPoint parent = null)
        {
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
