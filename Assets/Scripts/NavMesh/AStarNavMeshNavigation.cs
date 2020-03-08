using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Pieter.NavMesh
{
    public class AStarPoint
    {
        public Vertex vert;
        public float g;
        public float h;
        public float f;
        public AStarPoint parent;

        public override string ToString()
        {
            return vert.ToString() + " : " + g + " "+ h + " " + f ;
        }
    }

    public class AStarNavMeshNavigation
    {
        private const int numberOfSteps = 20;
        private NavMeshHolder navMesh;

        public AStarNavMeshNavigation(NavMeshHolder navMesh)
        {
            this.navMesh = navMesh;
        }

        public List<NavMeshMovementLine> GetPathFromTo(Vector3 from, Vector3 to)
        {
            NavMeshTriangle triStart = navMesh.GetContainingTriangle(from);
            if(triStart == null)
            {
                return null;
            }
            NavMeshTriangle triEnd = navMesh.GetContainingTriangle(to);
            if (triEnd == null)
            {
                return null;
            }
            List<NavMeshMovementLine> path = new List<NavMeshMovementLine>();

            // If the points are on the same triangle, send back a straight line between both points
            if (triStart.ID == triEnd.ID)
            {
                path.Add(new NavMeshMovementLine { point =from });
                path.Add(new NavMeshMovementLine { point = to });
                return path;
            }
            // TODO: Check if we are inside a triangle 
            List<AStarPoint> open = new List<AStarPoint>();
            // Add the starting triangle vertexes to the open list
            open.Add(CreateAStarPoint(0, 0, triStart.vertex1, to));
            open.Add(CreateAStarPoint(0, 0, triStart.vertex2, to));
            open.Add(CreateAStarPoint(0, 0, triStart.vertex3, to));

            List<AStarPoint> closed = new List<AStarPoint>();

            // The starting point does not need to be kept in the closed list

            while (open.Count > 0)
            {
                open.Sort((a, b) => a.f.CompareTo(b.f));
                AStarPoint current = open[0];

                if (current.vert.Equals(triEnd.vertex1) ||
                   current.vert.Equals(triEnd.vertex2) ||
                   current.vert.Equals(triEnd.vertex3))
                {
                    return ReconstructPath(current, to, from);
                }

                open.RemoveAt(0);
                closed.Add(current);
                if(open.Count > 4000)
                {
                    break;
                }
                foreach (Vertex vert in current.vert.adjacent)
                {
                    if (closed.Any(item => item.vert.Equals(vert)))
                    {
                        continue;
                    }

                    float cost = current.g + (current.vert.Position - vert.Position).magnitude;
                    AStarPoint aStarPoint = open.Find((item) => item.vert.Equals(vert));
                    if (aStarPoint == null)
                    {
                        aStarPoint = CreateAStarPoint(current.g, (current.vert.Position - vert.Position).magnitude, vert, to, current);

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

        private List<NavMeshMovementLine> ReconstructPath(AStarPoint lastPoint, Vector3 end, Vector3 start)
        {
            List<NavMeshMovementLine> path = new List<NavMeshMovementLine>();
            path.Add(new NavMeshMovementLine { point = end });
            path.Add(new NavMeshMovementLine { point = lastPoint.vert.Position });
            AStarPoint parent = lastPoint.parent;
            while(parent != null)
            {
                path.Add(new NavMeshMovementLine { point = parent.vert.Position });
                lastPoint = parent;
                parent = lastPoint.parent;
            }
            path.Add(new NavMeshMovementLine { point = start });
            path.Reverse();
            path = SmoothPath(path);
            return path;
        }

        private List<NavMeshMovementLine> SmoothPath(List<NavMeshMovementLine> path)
        {
            Vector3 lastPoint = path[0].point;
            foreach (NavMeshMovementLine line in path)
            {
                Debug.DrawLine(lastPoint, line.point, Color.magenta);
                lastPoint = line.point;
            }
            if (path.Count <= 2)
            {
                return path;
            }
            NavMeshTriangle triangle = null;
            for (int i = 0; i < path.Count - 2; i++)
            {
                if (path[i] == null)
                {
                    continue;
                }
                NavMeshMovementLine currentPoint = path[i];
                for (int j = i + 2; j - (i + 2) <= 5 && j < path.Count; j++)
                {
                    if (path[j] == null)
                    {
                        continue;
                    }
                    bool canRemove = true;
                    Vector3 direction = (path[j].point - currentPoint.point).normalized * 0.3f;
                    Vector3 nextStep = Vector3.Lerp(currentPoint.point, path[j].point, 0);
                    Debug.DrawLine(path[j].point, currentPoint.point, Color.cyan);
                    for (int k = 1; k < numberOfSteps; k++)
                    {
                        Debug.DrawLine(nextStep+Vector3.right, nextStep, new Color(0, k / (float)numberOfSteps, k / (float)numberOfSteps));
                        triangle = navMesh.GetContainingTriangle(nextStep);
                        if (triangle == null)
                        {
                            canRemove = false;
                            break;
                        }
                        nextStep = Vector3.Lerp(currentPoint.point, path[j].point, k / (float)numberOfSteps);
                    }
                    if (canRemove)
                    {
                        path[j - 1] = null;
                    }
                }
            }
            List<NavMeshMovementLine> newPath = new List<NavMeshMovementLine>();
            path.ForEach((item) => { if (item != null) newPath.Add(item); });
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
                Debug.Log(angle + " " + beforeDirAngle);
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
            float h = (end - current.Position).magnitude;
            float g = oldG + distance;
            return new AStarPoint() { vert = current, g = g, h = h, f = h+g, parent = parent };
        }
    }
}
