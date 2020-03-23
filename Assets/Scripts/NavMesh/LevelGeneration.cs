using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pieter.NavMesh;
using System;
using Random = UnityEngine.Random;
using System.Threading;
using Pieter.GraphTraversal;

public class LevelGeneration : MonoBehaviour
{
    public static Thread mainThread = Thread.CurrentThread;
    [SerializeField] private TraversalGraphHolder traversalGraphHolder = null;

    [SerializeField] private RoomInformation initialRoom = null;
    [SerializeField] private RoomInformation[] roomPrefabs = null;

    [SerializeField] private int numberOfRooms = 2;

    private List<NavMeshEntrance> availableEntrances = new List<NavMeshEntrance>();
    [SerializeField] private Transform test = null;

    [SerializeField] private Transform test2 = null;
    Vector3 a;
    Vector3 b;
    Vector3 c;
    Vector3 d;
    private IEnumerator Start()
    {
        RoomInformation initRoom = Instantiate<RoomInformation>(initialRoom, this.transform);
        //navMeshHolder.AddTriangles(initRoom.NavMeshGenerator);
        //navMeshHolder.AddVertexes(initRoom.NavMeshGenerator);
        traversalGraphHolder.AddTraversalLines(initRoom.TraversalGenerator);
        availableEntrances.AddRange(initRoom.GetAllEntrances());
        initRoom.SetID(0);

        int roomCounter = 0;
        int stopCounter = 4000;
        while(roomCounter < numberOfRooms && stopCounter++ > 0)
        {
            if (availableEntrances.Count == 0)
            {
                break;
            }
            // Pick a random entrance point
            int randomEntranceIndex = Random.Range(0, availableEntrances.Count);
            NavMeshEntrance randomEntrance = availableEntrances[randomEntranceIndex];
            a = randomEntrance.entranceMidPoint.position;
            b = randomEntrance.awayFromDoorDirection;
            // Choose a random generator
            RoomInformation randomRoomPrefab = GetRandomRoom();
            int randomRoomEntranceIndex = Random.Range(0, randomRoomPrefab.GetAllEntrances().Length);
            NavMeshEntrance randomRoomEntrance = randomRoomPrefab.GetEntrance(randomRoomEntranceIndex);

            float angle = Vector3.SignedAngle(randomEntrance.awayFromDoorDirection, randomRoomEntrance.awayFromDoorDirection, Vector3.up);
            float amountToRotate = 180 - angle;

            Collider[] col = Physics.OverlapBox(randomEntrance.entranceMidPoint.position + Quaternion.AngleAxis(amountToRotate + 180, Vector3.up) * (-randomRoomPrefab.center + randomRoomEntrance.entranceMidPoint.position), randomRoomPrefab.extents / 2);
            c = randomEntrance.entranceMidPoint.position + Quaternion.AngleAxis(amountToRotate + 180, Vector3.up) * (-randomRoomPrefab.center + randomRoomEntrance.entranceMidPoint.position);
            d = randomRoomPrefab.extents / 2;
            if (col.Length == 0)
            {
                // Init the random generator
                RoomInformation Room2 = Instantiate<RoomInformation>(randomRoomPrefab, this.transform);
                Room2.SetID(roomCounter+1);
                List<NavMeshEntrance> newRoomEntrances = new List<NavMeshEntrance>(Room2.GetAllEntrances());
                NavMeshEntrance Room2Entrance = newRoomEntrances[randomRoomEntranceIndex];
                newRoomEntrances.RemoveAt(randomRoomEntranceIndex);
                availableEntrances.AddRange(newRoomEntrances);

                Room2.transform.RotateAround(Room2Entrance.entranceMidPoint.position, Vector3.up, amountToRotate);
                Vector3 directionToMove = randomEntrance.entranceMidPoint.position - Room2Entrance.entranceMidPoint.position;
                Room2.transform.Translate(directionToMove, Space.World);

                //Room2.NavMeshGenerator.CreateTriangle(randomEntrance.entrance1, randomEntrance.entrance2, Room2Entrance.entrance1);
                //Room2.NavMeshGenerator.CreateTriangle(Room2Entrance.entrance1, Room2Entrance.entrance2, randomEntrance.entrance1);

                Pieter.GraphTraversal.TraversalEntrance traversalEntranceRoom2 =
                    Room2.TraversalGenerator.GetEntrance(Room2Entrance.ID);
                Pieter.GraphTraversal.TraversalEntrance traversalEntranceRandomRoom =
                    randomEntrance.generator.containedRoom.TraversalGenerator.GetEntrance(randomEntrance.ID);

                if (traversalEntranceRoom2 != null)
                {
                    Room2.TraversalGenerator.AddAdjacentNodes(traversalEntranceRoom2.vertex, traversalEntranceRandomRoom.vertex);
                    randomEntrance.generator.containedRoom.TraversalGenerator.AddAdjacentNodes(traversalEntranceRandomRoom.vertex, traversalEntranceRoom2.vertex);
                }
                else
                {
                    print("Could not find the door with ID " + Room2Entrance.ID + " on the door " + Room2.name);
                }
                //navMeshHolder.AddTriangles(Room2.NavMeshGenerator);
                //navMeshHolder.AddVertexes(Room2.NavMeshGenerator);
                traversalGraphHolder.AddTraversalLines(Room2.TraversalGenerator);

                availableEntrances.RemoveAt(randomEntranceIndex);

                roomCounter++;
            }
            yield return new WaitForEndOfFrame();
        }
        //navMeshHolder.Notify();
        traversalGraphHolder.Notify();
    }

    private RoomInformation GetRandomRoom()
    {
        return roomPrefabs[Random.Range(0, roomPrefabs.Length)];
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(a, 0.5f);
        Gizmos.DrawRay(a, b);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(c, 0.5f);
        Gizmos.DrawRay(c, d);

    }
}
