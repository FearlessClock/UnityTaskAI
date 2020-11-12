using System.Collections;
using System.Collections.Generic;
using Assets.Scripts;
using UnityEngine;
using Pieter.NavMesh;
using Pieter.GraphTraversal;

public class LODNavigationTester : MonoBehaviour
{
    [SerializeField] private TraversalGraphHolder traversalGraphHolder = null;

    private TraversalAStarNavigation graphNavigation = null;
    [SerializeField] private Transform end = null;
    private List<NavMeshMovementLine> path;

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
            startingRoom = traversalGraphHolder.GetClosestGenerator(end.position)?.containedRoom;
        }
        RoomInformation endingRoom = null;
        endingRoom = GetRoomInformationForLocation(end.position);
        if (endingRoom == null)
        {
            endingRoom = traversalGraphHolder.GetClosestGenerator(this.transform.position)?.containedRoom;
        }

        // ReSharper disable once ConditionIsAlwaysTrueOrFalse
        if (startingRoom != null && endingRoom != null)
            // ReSharper disable once HeuristicUnreachableCode
        {
            path = LevelOfDetailNavigationSolver.GetLODPath(this.transform.position, end.position,
                startingRoom, endingRoom, graphNavigation, true, true);
        }

    }

    private RoomInformation GetRoomInformationForLocation(Vector3 position)
    {       
        return traversalGraphHolder.GetClosestGenerator(position).containedRoom;
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
