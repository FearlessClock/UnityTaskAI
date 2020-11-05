using Pieter.GraphTraversal;
using Pieter.NavMesh;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;
using Random = UnityEngine.Random;

public class LevelGeneration : MonoBehaviour
{
    public static Thread mainThread = Thread.CurrentThread;
    [SerializeField] private TraversalGraphHolder traversalGraphHolder = null;

    [SerializeField] private RoomInformation initialRoom = null;
    [SerializeField] private RoomInformation[] roomPrefabs = null;

    [SerializeField] private int numberOfRooms = 2;

    [SerializeField] private LayerMask navMeshMask = 0;
    private List<NavMeshEntrance> availableEntrances = new List<NavMeshEntrance>();
    [SerializeField] private Transform test = null;

    [SerializeField] private Transform test2 = null;

    private GenerationMap generationMap = new GenerationMap(1);
    [SerializeField] private float startingAngle = 0;

    private List<RoomInformation> generatedRooms = new List<RoomInformation>();
    public List<RoomInformation> GeneratedRooms => generatedRooms;

    int roomCounter = 0;
    int stopCounter = 4000;
    private bool didNotify = false;
    Vector3 cen;
    Vector3 ext;

    [SerializeField] private RoomGraphHolder roomGraph = null;
    public Action OnDoneLevelGeneration = null;

    private void Start()
    {
        RoomInformation initRoom = Instantiate<RoomInformation>(initialRoom, this.transform);
        generatedRooms.Add(initRoom);
        traversalGraphHolder.AddTraversalLines(initRoom.TraversalGenerator);
        AddEntrancesToAvailableEntrances(initRoom.GetEntrances.ToList(), 0);
        initRoom.SetID(0);
        roomGraph.AddRoom(initRoom, null);
        generationMap.AddRectangle(initRoom.transform.position + initRoom.center, initRoom.extents);

    }

    private void OnDestroy()
    {
        roomGraph.Clear();
    }


    private void Update()
    {
        if (availableEntrances.Count == 0)
        {
            return;
        }
        if (roomCounter < numberOfRooms && stopCounter++ > 0)
        {
            // Pick a random entrance point
            int randomEntranceIndex = Random.Range(0, availableEntrances.Count);
            NavMeshEntrance randomEntrance = availableEntrances[randomEntranceIndex];
            // Choose a random generator
            RoomInformation randomRoomPrefab = GetRandomRoom();
            int randomRoomEntranceIndex = Random.Range(0, randomRoomPrefab.GetEntrances.Length);

            NavMeshEntrance randomRoomEntrance = randomRoomPrefab.GetEntrance(randomRoomEntranceIndex);

            float angle = Vector3.SignedAngle(randomEntrance.GetRotatedAwayFromDoorDirection(), randomRoomEntrance.GetRotatedAwayFromDoorDirection(), Vector3.up);
            float amountToRotate = 180 - angle;

            // Init the random generator
            RoomInformation room2 = Instantiate<RoomInformation>(randomRoomPrefab, this.transform);

            List<NavMeshEntrance> newRoomEntrances = new List<NavMeshEntrance>(room2.GetEntrances);
            NavMeshEntrance room2Entrance = newRoomEntrances[randomRoomEntranceIndex];

            room2.transform.RotateAround(room2Entrance.entranceMidPoint.transform.position, Vector3.up, amountToRotate);

            Vector3 directionToMove = randomEntrance.entranceMidPoint.transform.position - room2Entrance.entranceMidPoint.transform.position;
            room2.transform.Translate(directionToMove, Space.World);

            room2.GetRotatedCenter(out cen, out ext);
            cen += room2.transform.position;
            if (generationMap.IsRoomSpaceFree(cen, ext))
            {

                generationMap.AddRectangle(cen, ext);

                generatedRooms.Add(room2);

                room2.SetID(roomCounter + 1);
                roomGraph.AddRoom(room2, randomEntrance.generator.containedRoom);
                newRoomEntrances.RemoveAt(randomRoomEntranceIndex);

                AddEntrancesToAvailableEntrances(newRoomEntrances, amountToRotate);

                TraversalEntrance traversalEntranceRoom2 =
                        room2.TraversalGenerator.GetEntrance(room2Entrance.ID);
                TraversalEntrance traversalEntranceRandomRoom =
                    randomEntrance.generator.containedRoom.TraversalGenerator.GetEntrance(randomEntrance.ID);

                if (traversalEntranceRoom2 != null)
                {
                    room2.roomGrid.AddGrid(randomEntrance.generator.containedRoom.roomGrid, traversalEntranceRoom2.ID, traversalEntranceRandomRoom.ID);
                    randomEntrance.generator.containedRoom.roomGrid.AddGrid(room2.GetComponent<Pieter.Grid.RoomGrid>(), traversalEntranceRandomRoom.ID, traversalEntranceRoom2.ID);

                    room2.TraversalGenerator.AddAdjacentNodes(traversalEntranceRoom2.vertex, traversalEntranceRandomRoom.vertex);
                    randomEntrance.generator.containedRoom.TraversalGenerator.AddAdjacentNodes(traversalEntranceRandomRoom.vertex, traversalEntranceRoom2.vertex);

                    room2.AddConnectedRoom(randomEntrance.generator.containedRoom, room2Entrance);
                    randomEntrance.generator.containedRoom.AddConnectedRoom(room2, randomEntrance);

                    room2Entrance.connectedEntrance = randomEntrance;
                    randomEntrance.connectedEntrance = room2Entrance;
                }
                else
                {
                    print("Could not find the door with ID " + room2Entrance.ID + " on the door " + room2.name);
                }

                traversalGraphHolder.AddTraversalLines(room2.TraversalGenerator);

                availableEntrances.RemoveAt(randomEntranceIndex);
                Physics.SyncTransforms();
                roomCounter++;
            }
            else
            {
                Destroy(room2.gameObject);
            }

        }
        else
        {
            if (!didNotify)
            {
                didNotify = true;
                OnDoneLevelGeneration?.Invoke();
                traversalGraphHolder.Notify();
            }
        }
    }

    public Vector3 RotatePointAroundPivot(Vector3 point, Vector3 pivot, Vector3 angles)
    {
        return Quaternion.Euler(angles) * (point - pivot) + pivot;
    }

    private void AddEntrancesToAvailableEntrances(List<NavMeshEntrance> newRoomEntrances, float amountToRotate)
    {
        foreach (NavMeshEntrance entrance in newRoomEntrances)
        {
            entrance.spawnedRotation = amountToRotate;
        }

        availableEntrances.AddRange(newRoomEntrances);
    }

    private RoomInformation GetRandomRoom()
    {
        return roomPrefabs[Random.Range(0, roomPrefabs.Length)];
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(cen, 1);

        generationMap.DrawDebug(Color.yellow);
    }
}
