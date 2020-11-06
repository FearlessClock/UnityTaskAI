using System.Collections;
using System.Collections.Generic;
using Pieter.GraphTraversal;
using Pieter.NavMesh;
using UnityEngine;

public class TraversalTester : MonoBehaviour
{
    [SerializeField] private TraversalGraphHolder traversalHolder = null;
    private TraversalGenerator startingGenerator = null;
    private TraversalAStarNavigation navMesh = null;
    [SerializeField] private Transform end = null;
    [SerializeField] private bool getRandomPoint = false;
    private TraversalGenerator endingGenerator = null;
    List<NavMeshMovementLine> path;

    private Vector3 target;
    private TraversalLine selectedLine = null;

    private void Start()
    {
        startingGenerator = FindObjectOfType<TraversalGenerator>();
        navMesh = new TraversalAStarNavigation(traversalHolder);

        selectedLine = traversalHolder.GetRandomLine();

    }
    private void Update()
    {
        if (getRandomPoint)
        {
            selectedLine = traversalHolder.GetRandomLine();
        }

        endingGenerator = traversalHolder.GetClosestGenerator(end.position);
        startingGenerator = traversalHolder.GetClosestGenerator(this.transform.position);

        if (endingGenerator)
        {
            float closest = Vector3.Distance(end.position, endingGenerator.TraversalLines[0].vertex.Position);
            int index = 0;
            for (int i = 0; i < endingGenerator.TraversalLines.Length; i++)
            {
                if(closest > Vector3.Distance(end.position, endingGenerator.TraversalLines[i].vertex.Position))
                {
                    closest = Vector3.Distance(end.position, endingGenerator.TraversalLines[i].vertex.Position);
                    index = i;
                }
            }
            Vertex endVert = endingGenerator.TraversalLines[index].vertex;

            closest = Vector3.Distance(this.transform.position, startingGenerator.TraversalLines[0].vertex.Position);
            index = 0;
            for (int i = 0; i < startingGenerator.TraversalLines.Length; i++)
            {
                if (closest > Vector3.Distance(this.transform.position, startingGenerator.TraversalLines[i].vertex.Position))
                {
                    closest = Vector3.Distance(this.transform.position, startingGenerator.TraversalLines[i].vertex.Position);
                    index = i;
                }
            }
            path = navMesh.GetPathFromTo(startingGenerator.TraversalLines[index].vertex, endVert);
        }
    }

    private void OnDrawGizmos()
    {
        if (path != null)
        {
            Vector3 lastPoint = this.transform.position;
            foreach (NavMeshMovementLine item in path)
            {
                Gizmos.DrawLine(lastPoint + Vector3.up, item.point + Vector3.up);
                lastPoint = item.point;
            }
        }
    }
}