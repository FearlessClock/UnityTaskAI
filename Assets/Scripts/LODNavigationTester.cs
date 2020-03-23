using System.Collections;
using System.Collections.Generic;
using Assets.Scripts;
using UnityEngine;
using Pieter.NavMesh;
using Pieter.GraphTraversal;

public class LODNavigationTester : MonoBehaviour
{
    [SerializeField] private TraversalGraphHolder traversalGraphHolder = null;

    private AStarNavMeshNavigation navMesh = null;
    private TraversalAStarNavigation graphNavigation = null;
    [SerializeField] private Transform end = null;
    [SerializeField] private bool getRandomPoint = false;
    private List<NavMeshMovementLine> path;

    [SerializeField] private LayerMask roomsMask = 0;
    private Collider[] hits = new Collider[100];

    private void Start()
    {
        graphNavigation = new TraversalAStarNavigation(traversalGraphHolder);
    }
    private void Update()
    {
        RoomInformation startingRoom = null;
        startingRoom = GetRoomInformationForLocation(this.transform.position);
        if(startingRoom == null)
        {
            startingRoom = traversalGraphHolder.GetClosestGenerator(this.transform.position)?.containedRoom;
        }
        RoomInformation endingRoom = null;
        endingRoom = GetRoomInformationForLocation(end.position);
        if (endingRoom == null)
        {
            endingRoom = traversalGraphHolder.GetClosestGenerator(end.position)?.containedRoom;
        }

        // ReSharper disable once ConditionIsAlwaysTrueOrFalse
        if (startingRoom != null && endingRoom != null)
            // ReSharper disable once HeuristicUnreachableCode
        {
            path = LevelOfDetailNavigationSolver.GetLODPath(this.transform.position, end.position,
                startingRoom, endingRoom, graphNavigation);
        }

    }

    private RoomInformation GetRoomInformationForLocation(Vector3 position)
    {
        RoomInformation startingRoom = null;
        int nmbrOfHits = Physics.OverlapSphereNonAlloc(position, 1, hits, roomsMask);
        if (nmbrOfHits > 0)
        {
            for (int i = 0; i < nmbrOfHits; i++)
            {
                startingRoom = hits[i].GetComponent<RoomInformation>();
                if (startingRoom != null)
                {
                    break;
                }
            }
        }

        return startingRoom;
    }

    private void OnDrawGizmos()
    {
        if (path != null)
        {
            if(path.Count > 0)
            {
                Vector3 lastPoint = path[0].point;
                foreach (NavMeshMovementLine item in path)
                {
                    if (item != null)
                    {
                        Gizmos.DrawLine(lastPoint + Vector3.up, item.point + Vector3.up);
                        lastPoint = item.point;
                    }
                }
            }
        }
    }
}
