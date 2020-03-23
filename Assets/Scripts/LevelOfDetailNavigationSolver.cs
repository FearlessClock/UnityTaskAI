using System;
using System.Collections.Generic;
using Pieter.GraphTraversal;
using Pieter.NavMesh;
using UnityEngine;

namespace Assets.Scripts
{
    class LevelOfDetailNavigationSolver
    {
        public static List<NavMeshMovementLine> GetLODPath(Vector3 playerPosition, Vector3 endPosition, RoomInformation startingRoom, RoomInformation arrivalRoom, TraversalAStarNavigation graphNavigation)
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
            
            path.AddRange(startingRoom.NavMeshNavigation.GetPathFromTo(playerPosition, startingRoom.TraversalGenerator.MiddleVertex.Position));

            path.AddRange(graphNavigation.GetPathFromTo(
                startingRoom.TraversalGenerator.MiddleVertex, arrivalRoom.TraversalGenerator.MiddleVertex));
            path.AddRange(arrivalRoom.NavMeshNavigation.GetPathFromTo(arrivalRoom.TraversalGenerator.MiddleVertex.Position, endPosition));

            return path;
        }
    }
}
