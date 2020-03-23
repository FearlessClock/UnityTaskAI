using UnityEngine;

namespace Pieter.NavMesh
{
    [System.Serializable]
    public class NavMeshMovementLine
    {
        public Vector3 point;

        public override string ToString()
        {
            return point.ToString();
        }
    }
}
