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
    private TraversalGenerator gen = null;
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
        if (startingGenerator == null)
        {
            startingGenerator = FindObjectOfType<TraversalGenerator>();
        }
        if (getRandomPoint)
        {
            selectedLine = traversalHolder.GetRandomLine();
        }

        gen = traversalHolder.GetClosestGenerator(end.position);
        if (gen)
        {
            Vertex vert = traversalHolder.GetMiddleLineForCurrentGenerator(gen).vertex;
            path = navMesh.GetPathFromTo(traversalHolder.GetMiddleLineForCurrentGenerator(startingGenerator).vertex, vert);
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