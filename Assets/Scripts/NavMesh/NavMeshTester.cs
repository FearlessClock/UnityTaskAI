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
        [SerializeField] private float movementSpeed = 1;
        [SerializeField] private float rotationLerpAmount = 0.4f;

        private void Start()
        {
            navMesh = new AStarNavMeshNavigation(navMeshHolder);
            //SelectedTriangle.isSelected = false;
            SelectedTriangle = navMeshHolder.GetRandomTriangle();
            SelectedTriangle.isSelected = true;
            path = navMesh.GetPathFromTo(this.transform.position, getRandomPoint? navMeshHolder.GetRandomPointInTriangle(SelectedTriangle): end.position);
            if(path.Count > 0)
            {
                target = path[0].point;
                path.RemoveAt(0);
            }

        }
        private void Update()
        {
            if(getRandomPoint)
            {
                SelectedTriangle.isSelected = false;
                SelectedTriangle = navMeshHolder.GetRandomTriangle();
                SelectedTriangle.isSelected = true;
            }
            path = navMesh.GetPathFromTo(this.transform.position, getRandomPoint ? navMeshHolder.GetRandomPointInTriangle(SelectedTriangle) : end.position);
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
