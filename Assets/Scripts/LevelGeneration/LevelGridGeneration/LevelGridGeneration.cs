using Pieter.GraphTraversal;
using Pieter.NavMesh;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelGridGeneration : MonoBehaviour
{
    [SerializeField] private GridLevelSquareInformation[] levelBlocks = null;
    [SerializeField] private GridWorldMap gridWorldMap = null;
    [SerializeField] private int tileSize = 1;

    [SerializeField] private int numberOfRooms = 30;
    [Space]
    [SerializeField] private TraversalGraphHolder traversalGraphHolder = null;

    [SerializeField] private RoomInformation initialRoom = null;

    [SerializeField] private RoomGraphHolder roomGraph = null;
    public System.Action OnDoneLevelGeneration = null;


    private void Awake()
    {
        gridWorldMap = new GridWorldMap(tileSize);
        roomGraph.Clear();
        for (int i = 0; i < levelBlocks.Length; i++)
        {
            levelBlocks[i].RoomInfo.EntrancePoints.UpdateEntranceValues();
        }
        GenerateBuilding(numberOfRooms, numberOfRooms, new List<Vector2Int>() { new Vector2Int(0,0)});
    }

    private void GenerateBuilding(int maxBuildings, int numberOfBuildings, List<Vector2Int> availableSpots)
    {
        Vector2Int currentGridPoint;
        if (availableSpots.Count > 0)
        {
            int randomIndex = Random.Range(0, availableSpots.Count);
            currentGridPoint = availableSpots[randomIndex];
            availableSpots.RemoveAt(randomIndex);
        }
        else
        {
            return;
        }

        List<RoomInformation> neighborRoomsForFirstDirectionTest = GetNeighbouringRooms(currentGridPoint, new Vector2Int[4] { Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right });
        //Build the first room
        Vector3 pos = Vec2IntToVec3D(currentGridPoint * tileSize);
        int randomRoomIndex = GetWorkingRandomRoom(pos, neighborRoomsForFirstDirectionTest);


        if (randomRoomIndex < 0)
        {
            GenerateBuilding(maxBuildings, numberOfBuildings, availableSpots);
            return;
        }

        GridLevelSquareInformation room = Instantiate<GridLevelSquareInformation>(levelBlocks[randomRoomIndex], this.transform);
        room.RoomInfo.SetID(maxBuildings - numberOfBuildings);
        room.transform.position = pos;
        gridWorldMap.AddRectangle(currentGridPoint, Vector2Int.one, room.RoomInfo);

        roomGraph.AddRoom(room.RoomInfo);

        List<RoomInformation> surroundingRooms = GetNeighbouringRooms(currentGridPoint, room.RoomInfo.EntrancePoints.Directions);

        for (int i = 0; i < surroundingRooms.Count; i++)
        {
            roomGraph.AddChild(room.RoomInfo, surroundingRooms[i]);
        }
        foreach (RoomInformation neighbor in surroundingRooms)
        {
            Vector2 dir = GetSingleDirectionToNextRoom(room.RoomInfo.transform.position, neighbor.transform.position);

            NavMeshEntrance entranceRoom = room.RoomInfo.EntrancePoints.GetEntranceFromDirection(-dir);

            TraversalEntrance traversalEntranceRoom =
                    room.RoomInfo.TraversalGenerator.GetEntrance(entranceRoom.ID);

            if (traversalEntranceRoom != null)
            {
                NavMeshEntrance entranceNeighbor = neighbor.EntrancePoints.GetEntranceFromDirection(dir);

                TraversalEntrance traversalEntranceNeigbor =
                        neighbor.TraversalGenerator.GetEntrance(entranceNeighbor.ID);

                if (traversalEntranceNeigbor != null)
                {
                    room.RoomInfo.roomGrid.AddGrid(neighbor.roomGrid, entranceRoom.ID, traversalEntranceNeigbor.ID);
                    neighbor.roomGrid.AddGrid(room.RoomInfo.roomGrid, traversalEntranceNeigbor.ID, traversalEntranceRoom.ID);

                    room.RoomInfo.TraversalGenerator.AddAdjacentNodes(traversalEntranceRoom.vertex, traversalEntranceNeigbor.vertex);
                    neighbor.TraversalGenerator.AddAdjacentNodes(traversalEntranceNeigbor.vertex, traversalEntranceRoom.vertex);

                    room.RoomInfo.AddConnectedRoom(entranceNeighbor.generator.containedRoom, entranceRoom);
                    entranceNeighbor.generator.containedRoom.AddConnectedRoom(room.RoomInfo, entranceNeighbor);

                    entranceRoom.connectedEntrance = entranceNeighbor;
                    entranceNeighbor.connectedEntrance = entranceRoom;
                }
                else
                {
                    Debug.Log("-- Could not find entrance " + entranceNeighbor.ID);
                }
            }
            else
            {
                Debug.Log("-- Could not find entrance " + entranceRoom.ID);
            }
        }

        traversalGraphHolder.AddTraversalLines(room.RoomInfo.TraversalGenerator);

        numberOfBuildings--;
        if (numberOfBuildings > 0)
        {
            Vector2Int[] availableDirections = room.RoomInfo.EntrancePoints.Directions;
            foreach (Vector2Int dir in availableDirections)
            {
                if (!availableSpots.Contains(currentGridPoint + dir) && gridWorldMap.IsGridSpaceFree(currentGridPoint + dir, Vector2Int.one))
                {
                    availableSpots.Add(currentGridPoint + dir);
                }
            }

            GenerateBuilding(maxBuildings, numberOfBuildings, availableSpots);
        }
    }

    private int GetWorkingRandomRoom(Vector3 position, List<RoomInformation> neighborRoomsForFirstDirectionTest)
    {
        if(neighborRoomsForFirstDirectionTest.Count == 0)
        {
            return Random.Range(0, levelBlocks.Length);
        }
        bool doesThisBlockWork = false;
        int randomRoomIndex = 0;
        int tryCounter = 1000;
        while (!doesThisBlockWork && tryCounter > 0)
        {
            tryCounter--;
            randomRoomIndex = Random.Range(0, levelBlocks.Length);
            for (int i = 0; i < neighborRoomsForFirstDirectionTest.Count; i++)
            {
                Vector2 dir = GetSingleDirectionToNextRoom(position, neighborRoomsForFirstDirectionTest[i].transform.position);
                if (levelBlocks[randomRoomIndex].RoomInfo.EntrancePoints.HasEntranceInDirection(-dir))
                {
                    doesThisBlockWork = true;
                    Debug.Log("Found working room: " + position.ToString() + " " + -dir + " RandIndex " + randomRoomIndex);
                    break;
                }
            }
        }

        if(doesThisBlockWork)
        {
            return randomRoomIndex;
        }
        else
        {
            return -1;
        }
    }

    private static Vector2 GetSingleDirectionToNextRoom(Vector3 startingPosition, Vector3 endingPosition)
    {
        Vector2 dir = new Vector2((startingPosition - endingPosition).x, (startingPosition - endingPosition).z);
        dir.Normalize();

        if (Mathf.Abs(dir.x) > Mathf.Abs(dir.y))
        {
            dir = new Vector2(1 * Mathf.Sign(dir.x), 0);
        }
        else
        {
            dir = new Vector2(0, 1 * Mathf.Sign(dir.y));
        }

        return dir;
    }

    private List<RoomInformation> GetNeighbouringRooms(Vector2Int currentGridPoint, Vector2Int[] directions)
    {
        List<RoomInformation> surroundingRooms = new List<RoomInformation>();
        for (int i = 0; i < directions.Length; i++)
        {
            RoomInformation roomAtPos = gridWorldMap.At(currentGridPoint + directions[i]);
            if (roomAtPos)
            {
                surroundingRooms.Add(roomAtPos);
            }
        }

        return surroundingRooms;
    }

    private Vector3 Vec2IntToVec3D(Vector2Int vec)
    {
        return new Vector3(vec.x, 0, vec.y);
    }

    private void OnDrawGizmos()
    {
        if(gridWorldMap != null)
        {
            gridWorldMap.DrawDebug(Color.green);
        }
        if(roomGraph != null)
        {
            roomGraph.DebugGraph();
        }
    }
}
