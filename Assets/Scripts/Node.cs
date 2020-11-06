using UnityEngine;

[System.Serializable]
public class Node
{
    public int id;
    [HideInInspector]
    public Object payload;
    [HideInInspector]
    public Node[] connectedNodes = new Node[0];

    public void AddNode(Node graph)
    {
        if (connectedNodes == null)
        {
            connectedNodes = new Node[0];
        }
        Node[] graphNew = new Node[connectedNodes.Length + 1];
        for (int i = 0; i < connectedNodes.Length; i++)
        {
            graphNew[i] = connectedNodes[i];
        }
        graphNew[graphNew.Length - 1] = graph;
        connectedNodes = graphNew;
    }
}
