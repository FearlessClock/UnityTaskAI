﻿using Pieter.GraphTraversal;
using Pieter.Grid;
using Pieter.NavMesh;
using Save;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class LevelGridGeneration : MonoBehaviour
{
    [SerializeField] private RoomPrefabsList levelBlocks = null;
    [SerializeField] private GridLevelSquareInformation entranceBlock = null;

    Dictionary<int, GridLevelSquareInformation> roomPrefabsIds = new Dictionary<int, GridLevelSquareInformation>();
    [SerializeField] private GridMapHolder gridWorldHolder = null;
    [SerializeField] private RoomGridHolder roomGridHolder = null;
    private GridWorldMap gridWorldMap => gridWorldHolder.gridWorldMap;
    [SerializeField] private int tileSize = 1;

    [SerializeField] private int numberOfRooms = 30;
    [Space]
    [SerializeField] private TraversalGraphHolder traversalGraphHolder = null;

    [SerializeField] private RoomGraphHolder roomGraph = null;
    public System.Action OnDoneLevelGeneration = null;

    [SerializeField] private FloatVariable maxPositionX;
    [SerializeField] private FloatVariable maxPositionY;
    [SerializeField] private FloatVariable minPositionX;
    [SerializeField] private FloatVariable minPositionY;

    private List<GridLevelSquareInformation> generatedRooms = new List<GridLevelSquareInformation>();
    public GridLevelSquareInformation[] GeneratedRooms => generatedRooms.ToArray();

    public int MaxBuildings => numberOfRooms;
    private int buildingIdCounter = 0;
    private List<Vector2Int> availableSpots = new List<Vector2Int>();
    public Vector2Int[] GetAvailablePositions => availableSpots.ToArray();

    public int GetTileSize => tileSize;

    private void Awake()
    {
        roomPrefabsIds.Add(entranceBlock.blockID, entranceBlock);
        for (int i = 0; i < levelBlocks.Length; i++)
        {
            roomPrefabsIds.Add(levelBlocks.gridLevels[i].blockID, levelBlocks.gridLevels[i]);
        }
    }

    public void GenerateLevel()
    {
        ResetValues();
        availableSpots.Add(new Vector2Int(0, 0));
        StartCoroutine(GenerateBuilding(numberOfRooms, numberOfRooms, availableSpots));
    }

    public void GenerateLevel(Lab lab)
    {
        ResetValues();
        StartCoroutine(GenerateBuilding(lab, 0));
    }

    private void ResetValues()
    {
        maxPositionX.SetValue(0);
        maxPositionY.SetValue(0);
        minPositionX.SetValue(0);
        minPositionY.SetValue(0);
        gridWorldHolder.gridWorldMap = new GridWorldMap(tileSize);
        roomGridHolder.Reset();
        roomGraph.Clear();
        for (int i = generatedRooms.Count - 1; i >= 0; i--)
        {
            Destroy(generatedRooms[i].gameObject);
            generatedRooms.RemoveAt(i);
        }
        UpdateEntrancePointsForLevelBlocks();
    }

    private void UpdateEntrancePointsForLevelBlocks()
    {
        for (int i = 0; i < levelBlocks.Length; i++)
        {
            levelBlocks.gridLevels[i].RoomInfo.EntrancePoints.UpdateEntranceValues();
        }
    }

    //Regenerate the building from the save data
    private IEnumerator GenerateBuilding(Lab lab, int index)
    { 
        Vector2Int currentGridPoint = new Vector2Int((int)lab.blocks[index].x / tileSize, (int)lab.blocks[index].y / tileSize);
        int numberOfBuildingsBuilt = MaxBuildings - index;
        availableSpots.Clear();
        availableSpots.AddRange(lab.availableSpots);
        List<RoomInformation> neighborRoomsForFirstDirectionTest = GetNeighbouringRooms(currentGridPoint, new Vector2Int[4] { Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right });
        //Build the first room
        Vector3 pos = Vec2IntToVec3D(currentGridPoint * tileSize);
        GridLevelSquareInformation room = GenerateRoom(lab.maxBuildings, numberOfBuildingsBuilt, currentGridPoint, pos, roomPrefabsIds[lab.blocks[index].ID]);
        room.gridPoint = currentGridPoint;
        generatedRooms.Add(room);

        if (index+1 < lab.blocks.Count)
        {
            yield return GenerateBuilding(lab, index+1);
        }
        OnDoneLevelGeneration?.Invoke();
    }

    private IEnumerator GenerateBuilding(int maxBuildings, int numberOfBuildings, List<Vector2Int> availablePositions)
    {
        Vector2Int currentGridPoint;
        if (availablePositions.Count > 0)
        {
            int randomIndex = Random.Range(0, availablePositions.Count);
            currentGridPoint = availablePositions[randomIndex];
            availablePositions.RemoveAt(randomIndex);

            List<RoomInformation> neighborRoomsForFirstDirectionTest = GetNeighbouringRooms(currentGridPoint, new Vector2Int[4] { Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right });
            //Build the first room
            Vector3 pos = Vec2IntToVec3D(currentGridPoint * tileSize);
            GridLevelSquareInformation randomRoom = GetWorkingRandomRoom(pos, neighborRoomsForFirstDirectionTest);
            if (randomRoom == null)
            {
                yield return GenerateBuilding(maxBuildings, numberOfBuildings, availablePositions);
            }
            else
            {
                numberOfBuildings = CreateRoom(maxBuildings, numberOfBuildings, availablePositions, currentGridPoint, pos, randomRoom);

                if (numberOfBuildings > 0)
                {
                    yield return GenerateBuilding(maxBuildings, numberOfBuildings, availablePositions);
                }
            }
        }
        OnDoneLevelGeneration?.Invoke();
    }

    public void CreateNewRoom(Vector2Int roomGridPoint, GridLevelSquareInformation room)
    {
        availableSpots.Remove(roomGridPoint);
        Vector3 pos = Vec2IntToVec3D(roomGridPoint * tileSize);
        numberOfRooms = CreateRoom(MaxBuildings, numberOfRooms, availableSpots, roomGridPoint, pos, room);
    }

    private int CreateRoom(int maxBuildings, int numberOfBuildings, List<Vector2Int> availablePositions, Vector2Int currentGridPoint, Vector3 pos, GridLevelSquareInformation randomRoom)
    {
        GridLevelSquareInformation room = GenerateRoom(maxBuildings, numberOfBuildings, currentGridPoint, pos, randomRoom);
        room.gridPoint = currentGridPoint;
        generatedRooms.Add(room);
        numberOfBuildings--;

        Vector2Int[] availableDirections = room.RoomInfo.EntrancePoints.Directions;
        foreach (Vector2Int dir in availableDirections)
        {
            if (!availablePositions.Contains(currentGridPoint + dir) && gridWorldMap.IsGridSpaceFree(currentGridPoint + dir, Vector2Int.one))
            {
                availablePositions.Add(currentGridPoint + dir);
            }
        }

        return numberOfBuildings;
    }

    private GridLevelSquareInformation GenerateRoom(int maxBuildings, int numberOfBuildings, Vector2Int gridPosition, Vector3 worldSpacePosition, GridLevelSquareInformation randomRoom)
    {
        if (worldSpacePosition.x >= maxPositionX.value )
        {
            maxPositionX.SetValue(worldSpacePosition.x + tileSize);
        }
        else if( worldSpacePosition.x < minPositionX.value)
        {
            minPositionX.SetValue(worldSpacePosition.x);
        }
        
        if (worldSpacePosition.y >= maxPositionY.value )
        {
            maxPositionY.SetValue(worldSpacePosition.y + tileSize);
        }
        else if( worldSpacePosition.y < minPositionY.value)
        {
            minPositionY.SetValue(worldSpacePosition.y);
        }

        GridLevelSquareInformation room = Instantiate<GridLevelSquareInformation>(randomRoom, this.transform);
        room.RoomInfo.SetID(buildingIdCounter++);
        room.transform.position = worldSpacePosition;
        gridWorldMap.AddRectangle(gridPosition, Vector2Int.one, room.RoomInfo);

        roomGraph.AddRoom(room.RoomInfo);
        room.RoomInfo.roomGrid.Initialize(worldSpacePosition, Quaternion.identity);
        roomGridHolder.AddRoom(room.RoomInfo.roomGrid);

        List<RoomInformation> surroundingRooms = GetNeighbouringRooms(gridPosition, room.RoomInfo.EntrancePoints.Directions);

        foreach (RoomInformation neighbor in surroundingRooms)
        {
            Vector2 dir = GetSingleDirectionToNextRoom(room.RoomInfo.transform.position, neighbor.transform.position);

            NavMeshEntrance entranceRoom = room.RoomInfo.EntrancePoints.GetEntranceFromDirection(-dir);

            TraversalEntrance traversalEntranceRoom =
                    room.RoomInfo.TraversalGenerator.GetEntrance(entranceRoom.ID);

            if (traversalEntranceRoom != null)
            {
                NavMeshEntrance entranceNeighbor = neighbor.EntrancePoints.GetEntranceFromDirection(dir);
                if (entranceNeighbor != null)
                {
                    TraversalEntrance traversalEntranceNeigbor =
                           neighbor.TraversalGenerator.GetEntrance(entranceNeighbor.ID);

                    if (traversalEntranceNeigbor != null)
                    {
                        ConnectCurrentRoomToNeighborRoom(room.RoomInfo, neighbor, entranceRoom, traversalEntranceRoom, entranceNeighbor, traversalEntranceNeigbor);
                    }
                    else
                    {
                        Debug.Log("-- Could not find entrance " + entranceNeighbor.ID + " on " + entranceNeighbor.generator.transform.parent.name);
                    }
                }
            }
            else
            {
                Debug.Log("-- Could not find entrance " + entranceRoom.ID + " In direction " + dir + " From room " + room.RoomInfo.ID);
            }
        }

        traversalGraphHolder.AddTraversalLines(room.RoomInfo.TraversalGenerator);
        return room;
    }

    private void ConnectCurrentRoomToNeighborRoom(RoomInformation room, RoomInformation neighbor, NavMeshEntrance entranceRoom, TraversalEntrance traversalEntranceRoom, NavMeshEntrance entranceNeighbor, TraversalEntrance traversalEntranceNeigbor)
    {
        roomGraph.AddChild(room, neighbor);

        room.roomGrid.AddGrid(neighbor.roomGrid, entranceRoom.ID, traversalEntranceNeigbor.ID);
        neighbor.roomGrid.AddGrid(room.roomGrid, traversalEntranceNeigbor.ID, entranceRoom.ID);

        //neighbor.TraversalGenerator.RemoveNode(traversalEntranceNeigbor.vertex);
        //room.TraversalGenerator.AddAdjacentNodes(traversalEntranceRoom.vertex, traversalEntranceNeigbor.vertex);
        //neighbor.TraversalGenerator.AddAdjacentNodes(traversalEntranceNeigbor.vertex, traversalEntranceRoom.vertex);

        room.AddConnectedRoom(entranceNeighbor.generator.containedRoom, entranceRoom);
        entranceNeighbor.generator.containedRoom.AddConnectedRoom(room, entranceNeighbor);

        entranceNeighbor.IsUsed = true;
        entranceRoom.IsUsed = true;

        room.TraversalGenerator.FuseNode(traversalEntranceRoom.vertex, traversalEntranceNeigbor.vertex, neighbor.TraversalGenerator, neighbor.EntrancePoints);

        entranceRoom.connectedEntrance = entranceNeighbor;
        entranceNeighbor.connectedEntrance = entranceRoom;


    }

    private GridLevelSquareInformation GetWorkingRandomRoom(Vector3 position, List<RoomInformation> neighborRoomsForFirstDirectionTest)
    {
        if(neighborRoomsForFirstDirectionTest.Count == 0)
        {
            return entranceBlock;
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
                if (levelBlocks.gridLevels[randomRoomIndex].RoomInfo.EntrancePoints.HasEntranceInDirection(-dir))
                {
                    doesThisBlockWork = true;
                    break;
                }
            }
        }

        if(doesThisBlockWork)
        {
            return levelBlocks.gridLevels[randomRoomIndex];
        }
        else
        {
            return null;
        }
    }

    private static Vector2 GetSingleDirectionToNextRoom(Vector3 startingPosition, Vector3 endingPosition)
    {
        Vector2 dir = new Vector2((startingPosition - endingPosition).x, (startingPosition - endingPosition).y);
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

    public static Vector3 Vec2IntToVec3D(Vector2Int vec)
    {
        return new Vector3(vec.x, vec.y, 0);
    }
    private void OnDrawGizmos()
    {
        Gizmos.DrawLine(new Vector3(minPositionX.value, minPositionY.value, 0), new Vector3(minPositionX.value, maxPositionY.value, 0));
        Gizmos.DrawLine(new Vector3(maxPositionX.value, maxPositionY.value, 0), new Vector3(minPositionX.value, maxPositionY.value, 0));
        Gizmos.DrawLine(new Vector3(maxPositionX.value, maxPositionY.value, 0), new Vector3(maxPositionX.value, minPositionY.value, 0));
        Gizmos.DrawLine(new Vector3(maxPositionX.value, minPositionY.value, 0), new Vector3(minPositionX.value, minPositionY.value, 0));
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
