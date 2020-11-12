using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomEntranceTester : MonoBehaviour
{
    [SerializeField] private RoomInformation foundRoom = null;
    [SerializeField] private GridMapHolder gridWorldHolder = null;
    private Vector3 foundEntrance;
    private Vector3 dir;

    private void Update()
    {
        foundRoom = gridWorldHolder.GetRoomAtWorldPosition(this.transform.position);
        dir = ((foundRoom.center + foundRoom.transform.position) - this.transform.position).normalized;

        if (Mathf.Abs(dir.x) > Mathf.Abs(dir.z))
        {
            dir = new Vector2(1 * Mathf.Sign(dir.x), 0);
        }
        else
        {
            dir = new Vector2(0, 1 * Mathf.Sign(dir.z));
        }
        foundEntrance = foundRoom.EntrancePoints.GetEntranceFromDirection(-dir).entrance.Position;
    }

    private void OnDrawGizmos()
    {
        if (foundRoom)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(foundRoom.center + foundRoom.transform.position, new Vector3(dir.x, 0, dir.y) * 5 + foundRoom.center + foundRoom.transform.position);
            Gizmos.DrawSphere(foundEntrance, 0.6f);
        }
    }
}
