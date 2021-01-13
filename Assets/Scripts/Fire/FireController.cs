using System;
using System.Collections;
using System.Collections.Generic;
using Pieter.Grid;
using Pieter.NavMesh;
using UnityEngine;
using Random = UnityEngine.Random;

public class FireController : MonoBehaviour
{
    private Vector2Int fireStartPoint;
    [SerializeField] private GameObject firePrefab = null;
    private List<FireBlock> currentFirePositions = new List<FireBlock>();
    private List<FireBlock> adjacentGridPoints = new List<FireBlock>();
    private List<FireBlock> finishedBurningPoints = new List<FireBlock>();

    [SerializeField] private float fuel = 10;
    [SerializeField] private float varianceFuel = 1;
    [SerializeField] private float fireResistance = 10;
    [SerializeField] private float variancefireResistance = 5;
    [SerializeField] private float smolderTime = 10;
    [SerializeField] private float varianceSmolder = 5;
    [Range(0,1)]
    [SerializeField] private float chanceToStartNewFire = 0.3f;

    [SerializeField] private LevelGridGeneration gridGeneration = null;
    private bool fireIsSpreading = false;
    [SerializeField] private float burnRate = 0.1f;
    [SerializeField] private RoomGridHolder roomGridHolder = null;
    [SerializeField] private RoomGraphHolder roomGraphHolder = null;
    private bool isCalculatingFire = false;
    private List<bool[]> roomsOnFire = null;

    public void Start()
    {
        gridGeneration = FindObjectOfType<LevelGridGeneration>();
        if (gridGeneration)
        {
            gridGeneration.OnDoneLevelGeneration += GetWorldLayout;
        }
        GetWorldLayout();
        currentFirePositions.Clear();
        adjacentGridPoints.Clear();
    }

    public RoomInformation GetRandomRoom()
    {
        return roomGraphHolder.rooms[Random.Range(0, roomGraphHolder.rooms.Length)].payload as RoomInformation;
    }

    public void GetWorldLayout()
    {
        roomsOnFire = new List<bool[]>();
        for (int i = 0; i < roomGridHolder.rooms.Count; i++)
        {
            roomsOnFire.Add(new bool[roomGridHolder.rooms[i].GetTotalGridPoints]);
        }
    }

    public void StartFire(Vector2Int fireStartPos, RoomInformation room)
    {
        fireStartPoint = fireStartPos;
        Debug.Log("Fire started at " + room.ID + " " + fireStartPoint);
        fireIsSpreading = true;
        GridPoint startpoint = roomGridHolder.GetGridPoint(fireStartPoint, room);
        CreateNewFirePoint(new FireBlock() { currentOxygen = 10, currentFuel = fuel + Random.Range(-varianceFuel, varianceFuel), maxFuel = fuel, gridPoint = startpoint, fireResistance = 0, isOnFire = true, smolderTime = smolderTime + Random.Range(-varianceSmolder, varianceSmolder) });
        //Instantiate(firePrefab, startpoint.center, room.transform.rotation, this.transform);
    }

    private IEnumerator UpdateFire()
    {
        isCalculatingFire = true;
        for (int i = currentFirePositions.Count - 1; i >= 0; i--)
        {
            currentFirePositions[i].Burn(burnRate);
            if (currentFirePositions[i].IsBurnedOut)
            {
                finishedBurningPoints.Add(currentFirePositions[i]);
                currentFirePositions.RemoveAt(i);
            }
            if (i % 10000 == 0)
            {
                yield return new WaitForEndOfFrame();
            }
        }
        for (int i = adjacentGridPoints.Count - 1; i >= 0; i--)
        {
            adjacentGridPoints[i].Burn(burnRate);
            if (adjacentGridPoints[i].CanLightOnFire)
            {
                CreateNewFirePoint(adjacentGridPoints[i]);
                adjacentGridPoints.RemoveAt(i);
            }
            if (i % 10000 == 0)
            {
                yield return new WaitForEndOfFrame();
            }
        }
        for (int i = finishedBurningPoints.Count - 1; i >= 0; i--)
        {
            finishedBurningPoints[i].Smolder(burnRate);
            if (finishedBurningPoints[i].IsSmoldered)
            {
                RemoveRoomOnFire(finishedBurningPoints[i]);
                finishedBurningPoints.RemoveAt(i);
            }
            if (i % 10000 == 0)
            {
                yield return new WaitForEndOfFrame();
            }
        }
        isCalculatingFire = false;
    }

    private void CreateNewFirePoint(FireBlock point)
    {
        if(!roomsOnFire[point.gridPoint.roomgrid.ID][TwoDToOneD(point.gridPoint.gridPosition, point.gridPoint.roomgrid)])
        {
            point.isOnFire = true;
            currentFirePositions.Add(point);
            GridPoint[] adjacentRooms = point.gridPoint.GetAllAdjacentGridPoints();
            for (int i = 0; i < adjacentRooms.Length; i++)
            {
                if(!roomsOnFire[point.gridPoint.roomgrid.ID][TwoDToOneD(point.gridPoint.gridPosition, point.gridPoint.roomgrid)] && adjacentRooms[i].isInside && 
                    Random.Range(0f, 1f) < chanceToStartNewFire)
                {
                    adjacentGridPoints.Add(new FireBlock() { fireResistance = fireResistance + Random.Range(-variancefireResistance, variancefireResistance), currentOxygen = 10, currentFuel = fuel + Random.Range(-varianceFuel, varianceFuel), maxFuel = fuel, gridPoint = adjacentRooms[i], smolderTime = smolderTime + Random.Range(-varianceSmolder, varianceSmolder) });
                }
            }
            roomsOnFire[point.gridPoint.roomgrid.ID][TwoDToOneD(point.gridPoint.gridPosition, point.gridPoint.roomgrid)] = true;
        }
    }

    private void RemoveRoomOnFire(FireBlock block)
    {
        roomsOnFire[block.gridPoint.roomgrid.ID][TwoDToOneD(block.gridPoint.gridPosition, block.gridPoint.roomgrid)] = false;
    }

    private int TwoDToOneD(Vector2Int gridPosition, RoomGrid room)
    {
        return room.GetGridWidth * gridPosition.y + gridPosition.x;
    }

    private void Update()
    {
        if(!isCalculatingFire && fireIsSpreading)
        {
            StartCoroutine(UpdateFire());
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        
        foreach (FireBlock point in adjacentGridPoints)
        {
            Gizmos.DrawSphere(point.gridPoint.center, 0.3f);
        }
        Gizmos.color = Color.red;
        foreach (FireBlock point in currentFirePositions)
        {
            Gizmos.DrawSphere(point.gridPoint.center, 0.3f);
        }
    }
}
