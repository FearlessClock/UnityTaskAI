using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AStarNodePoint
{
    public bool inClosed = false;
    public bool inOpen = false;
    public Node node;
    public float g;
    public float h;
    public float f;
    public AStarNodePoint parent;

    public override string ToString()
    {
        return node + " : " + g + " " + h + " " + f;
    }

    public override bool Equals(object obj)
    {
        return (obj as AStarNodePoint).node.id == node.id;
    }

    public override int GetHashCode()
    {
        int hashCode = 1859853986;
        hashCode = hashCode * -1521134295 + inClosed.GetHashCode();
        hashCode = hashCode * -1521134295 + EqualityComparer<Node>.Default.GetHashCode(node);
        hashCode = hashCode * -1521134295 + g.GetHashCode();
        hashCode = hashCode * -1521134295 + h.GetHashCode();
        hashCode = hashCode * -1521134295 + f.GetHashCode();
        hashCode = hashCode * -1521134295 + EqualityComparer<AStarNodePoint>.Default.GetHashCode(parent);
        return hashCode;
    }
}
public class AStarRoomGraphNavigation
{
    public delegate bool SpecialTraversalCheck(Node x, Node y);

    private const int numberOfSteps = 13;
    // GetPath Variables
    List<Node> path = new List<Node>();
    List<AStarNodePoint> open = new List<AStarNodePoint>();
    AStarNodePoint current;
    AStarNodePoint outValue;

    Dictionary<int, AStarNodePoint> actualPoints = new Dictionary<int, AStarNodePoint>();
    public AStarRoomGraphNavigation(Node[] nodes)
    {
        UpdateNodes(nodes);
    }

    public void UpdateNodes(Node[] nodes)
    {
        actualPoints = new Dictionary<int, AStarNodePoint>();
        for (int i = 0; i < nodes.Length; i++)
        {
            actualPoints.Add(nodes[i].id, new AStarNodePoint() { f = 0, g = 0, h = 0, inClosed = false, parent = null, node = nodes[i] });
        }
    }

    private AStarNodePoint GetPointValue(int index)
    {
        return actualPoints[index];
    }

    public List<Node> GetPathFromTo(Node from, Node to, SpecialTraversalCheck Check, bool keepStartingNode = true, bool keepEndingNode = true)
    {
        if (from == null)
        {
            return new List<Node>();
        }
        if (to == null)
        {
            return new List<Node>();
        }

        // If the nodes are the same node, send back a straight line between both nodes
        if (from.id == to.id)
        {
            if (keepStartingNode)
            {
                path.Add(from);
            }
            if (keepEndingNode)
            {
                path.Add(to);
            }
            return path;
        }


        ResetNodes();

        path.Clear();
        open.Clear();
        open.Add(UpdateAStarPoint(actualPoints[from.id], 0, 0, to.position));
        actualPoints[from.id].inOpen = true;

        // The starting point does not need to be kept in the closed list

        while (open.Count > 0)
        {
            open.Sort((a, b) => a.f.CompareTo(b.f));
            current = open[0];

            if (current.node.id == to.id)
            {
                return ReconstructPath(current, to, from, keepStartingNode, keepEndingNode);
            }

            open.RemoveAt(0);
            current.inOpen = false;
            current.inClosed = true;
            if (open.Count > 5000)
            {
                break;
            }
            for (int i = 0; i < current.node.connectedNodes.Length; i++)
            {
                AStarNodePoint aStarPointExisting = GetPointValue(current.node.connectedNodes[i].id);
                if(!Check(current.node, current.node.connectedNodes[i]))
                {
                    continue;
                }

                if (aStarPointExisting.inClosed)
                {
                    continue;
                }

                float cost = current.g + GetSquareDistance(current.node.position, current.node.connectedNodes[i].position);
                if (!aStarPointExisting.inOpen)
                {
                    aStarPointExisting = UpdateAStarPoint(aStarPointExisting, current.g, cost, to.position, current);
                    aStarPointExisting.inOpen = true;
                    open.Add(aStarPointExisting);
                }
                else
                {
                    if (cost < aStarPointExisting.g)
                    {
                        aStarPointExisting.g = cost;
                        aStarPointExisting.f = aStarPointExisting.g + aStarPointExisting.h;
                        aStarPointExisting.parent = current;
                    }
                }
            }
        }

        return path;
    }

    private void ResetNodes()
    {
        for (int i = 0; i < actualPoints.Count; i++)
        {
            actualPoints[i].inClosed = false;
            actualPoints[i].inOpen = false;
            actualPoints[i].f = 0;
            actualPoints[i].g = 0;
            actualPoints[i].h = 0;
            actualPoints[i].parent = null;
        }
    }

    private List<Node> reconstructedPath = new List<Node>();

    public int NumberOfRooms => actualPoints.Count;

    private List<Node> ReconstructPath(AStarNodePoint lastPoint, Node end, Node start, bool keepStartingNode, bool keepEndingNode)
    {
        reconstructedPath = new List<Node>();
        if (keepEndingNode)
        {
            reconstructedPath.Add(end);
        }
        reconstructedPath.Add(lastPoint.node);
        current = lastPoint.parent;
        while (current != null)
        {
            reconstructedPath.Add(current.node);
            lastPoint = current;
            current = lastPoint.parent;
        }
        if (keepStartingNode)
        {
            reconstructedPath.Add(start);
        }
        reconstructedPath.Reverse();
        return reconstructedPath;
    }

    private static AStarNodePoint UpdateAStarPoint(AStarNodePoint value, float oldG, float distance, Vector2 end, AStarNodePoint parent = null)
    {
        float h = GetSquareDistance(value.node.position, end);
        float g = oldG + distance;
        value.g = g;
        value.h = h;
        value.f = h + g;
        value.parent = parent;
        return value;
    }

    private static float GetSquareDistance(Vector2 value, Vector2 end)
    {
        return (Mathf.Pow((value.x - end.x), 2) + Mathf.Pow((value.y - end.y), 2));
    }
}

