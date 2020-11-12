using System;
using System.Collections.Generic;
using Pieter.GraphTraversal;
using Pieter.NavMesh;
using UnityEngine;

namespace Assets.Scripts
{
    class LevelOfDetailNavigationSolver
    {
        public static List<NavMeshMovementLine> GetLODPath(Vector3 playerPosition, Vector3 endPosition, RoomInformation startingRoom, RoomInformation arrivalRoom, TraversalAStarNavigation graphNavigation, bool keepStartingNode = false, bool keepEndingNode = true)
        {
            if (startingRoom == null || arrivalRoom == null)
            {
                return null;
            }

            if (startingRoom.Equals(arrivalRoom))
            {
                return startingRoom.NavMeshNavigation.GetPathFromTo(playerPosition, endPosition);
            }
            List<NavMeshMovementLine> path = new List<NavMeshMovementLine>();
            Vertex closestToPlayerTraversalVertex = startingRoom.TraversalGenerator.ClosestVertex(endPosition);
            Vertex closestToEndTraversalVertex = arrivalRoom.TraversalGenerator.ClosestVertex(playerPosition);
            List<NavMeshMovementLine> traversalMovementList = graphNavigation.GetPathFromTo(
               closestToPlayerTraversalVertex, closestToEndTraversalVertex);
            if(traversalMovementList.Count == 0)
            {
                return null;
            }

            path.AddRange(startingRoom.NavMeshNavigation.GetPathFromTo(playerPosition, closestToPlayerTraversalVertex.Position, keepStartingNode, false));
            
            path.AddRange(traversalMovementList);
            path.AddRange(arrivalRoom.NavMeshNavigation.GetPathFromTo(closestToEndTraversalVertex.Position, endPosition, false, keepEndingNode));

            return path;
        }
    }
}
