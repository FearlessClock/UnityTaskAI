using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Pieter.NavMesh
{
    public class NavMeshTester : MonoBehaviour
    {
        [SerializeField] private NavMesh.NavMeshHolder navMeshHolder = null;
        private NavMesh.AStarNavMeshNavigation navMesh = null;
        [SerializeField] private Transform end = null;
        [SerializeField] private bool getRandomPoint = false;
        List<NavMeshMovementLine> path;

        private Vector3 target;
        private NavMeshTriangle SelectedTriangle = null;

        private void Start()
        {

        }
        private void Update()   
        {
            if(getRandomPoint)
            {
                SelectedTriangle.isSelected = false;
                SelectedTriangle = navMeshHolder.GetRandomTriangle();
                if (SelectedTriangle != null)
                {
                    SelectedTriangle.isSelected = true;
                }
            }

            if(navMeshHolder != null)
            {
                if(navMesh == null)
                {
                    navMesh = new AStarNavMeshNavigation(navMeshHolder);
                }
                path = navMesh.GetPathFromTo(this.transform.position, getRandomPoint ? navMeshHolder.GetRandomPointInTriangle(SelectedTriangle) : end.position);
            }
        }

        private void OnDrawGizmos()
        {
            if (path != null)
            {
                Vector3 lastPoint = this.transform.position;
                foreach (NavMeshMovementLine item in path)
                {
                    Gizmos.DrawLine(lastPoint, item.point);
                    lastPoint = item.point;
                }
            }
        }
    }
}
