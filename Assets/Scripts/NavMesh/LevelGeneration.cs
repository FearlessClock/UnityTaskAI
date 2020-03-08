using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pieter.NavMesh;
public class LevelGeneration : MonoBehaviour
{
    [SerializeField] private NavMeshHolder navMeshHolder = null;
    
    [SerializeField] private RoomInformation roomPrefab = null;
    [SerializeField] private RoomInformation roomPrefab2 = null;

    [SerializeField] private int numberOfRooms = 2;



    private void Awake()
    {
        RoomInformation initRoom = Instantiate<RoomInformation>(roomPrefab, this.transform);
        NavMeshEntrance firstRoomEntrance = initRoom.GetEntrance(0);
        RoomInformation Room2 = Instantiate<RoomInformation>(roomPrefab2, this.transform);
        NavMeshEntrance Room2Entrance = Room2.GetEntrance(0);

        float angle = Vector3.SignedAngle(firstRoomEntrance.awayFromDoorDirection, Room2Entrance.awayFromDoorDirection, Vector3.up);
        float amountToRotate = 180 - angle;
        Room2.transform.RotateAround(Room2Entrance.entranceMidPoint.position, Vector3.up, amountToRotate);
        Vector3 directionToMove = firstRoomEntrance.entranceMidPoint.position - Room2Entrance.entranceMidPoint.position;
        Room2.transform.Translate(directionToMove, Space.World) ;
        initRoom.NavMeshGenerator.CreateTriangle(firstRoomEntrance.entrance1, firstRoomEntrance.entrance2, Room2Entrance.entrance1);
        initRoom.NavMeshGenerator.CreateTriangle(Room2Entrance.entrance1, Room2Entrance.entrance2, firstRoomEntrance.entrance1);

        navMeshHolder.CollectTriangles(new NavMeshGenerator[2] { initRoom.NavMeshGenerator, Room2.NavMeshGenerator });
    }
}
