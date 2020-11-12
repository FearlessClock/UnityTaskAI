using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class RoomFinderTester : MonoBehaviour
{
    [SerializeField] private RoomInformation foundRoom = null;
    [SerializeField] private GridMapHolder gridWorldHolder = null;
    [SerializeField] private Vector2Int currentGridPoint;
    List<RoomInformation> surroundingRooms;

    private void Update()
    {
        foundRoom = gridWorldHolder.GetRoomAtWorldPosition(this.transform.position);
        Vector2Int[] directions = new Vector2Int[4] { Vector2Int.up, Vector2Int.down, Vector2Int.right, Vector2Int.left };
        currentGridPoint = gridWorldHolder.GetGridPosition(this.transform.position);

        surroundingRooms = new List<RoomInformation>();
        Debug.Log("---------");
        Debug.Log(currentGridPoint);
        for (int i = 0; i < directions.Length; i++)
        {
            RoomInformation roomAtPos = gridWorldHolder.gridWorldMap.At(currentGridPoint + directions[i]);
            if (roomAtPos)
            {
                Debug.DrawLine(this.transform.position, roomAtPos.center + roomAtPos.transform.position);
                surroundingRooms.Add(roomAtPos);
            }
        }
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.gray;
        if(foundRoom != null)
        {
            Gizmos.DrawCube(foundRoom.center + foundRoom.transform.position, foundRoom.extents);
        }
        if(surroundingRooms != null)
        {
            Gizmos.color = Color.yellow;
            for (int i = 0; i < surroundingRooms.Count; i++)
            {
                Gizmos.DrawCube(surroundingRooms[i].center + surroundingRooms[i].transform.position, surroundingRooms[i].extents);
            }
        }
    }
}
