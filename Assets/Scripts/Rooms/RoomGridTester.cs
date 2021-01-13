using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomGridTester : MonoBehaviour
{
    [SerializeField] private PolygonCollider2D col = null;

    private void OnDrawGizmos()
    {
        if(col != null && col.OverlapPoint(this.transform.position.ThreeDTo2DVector()))
        {
            Gizmos.color = Color.red;
        }
        Gizmos.DrawSphere(this.transform.position, 1);
    }
}
