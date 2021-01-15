using System;
using System.Collections;
using System.Collections.Generic;
using Pieter.Grid;
using Pieter.NavMesh;
using UnityEngine;
using Random = UnityEngine.Random;

public enum FireState { NOTHING = 0, ADJACENT = 1, BURNING = 2, SMOLDERING = 3}
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
    private List<FireState[]> roomsOnFire = null; 
    [SerializeField] private int stepsToCheck = 2000;
    private int firePositionCurrentStep = 0;
    private int adjacentPositionCurrentStep = 0;
    private int SmolderCurrentStep = 0;
    private long currentTick = 0;

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
        roomsOnFire = new List<FireState[]>();
        for (int i = 0; i < roomGridHolder.rooms.Count; i++)
        {
            roomsOnFire.Add(new FireState[roomGridHolder.rooms[i].GetTotalGridPoints]);
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

    public void StartFire(Vector3 fireStartPos, RoomInformation room)
    {
        StartFire(room.roomGrid.GetIntPos(fireStartPos), room);
    }

    private IEnumerator UpdateFire()
    {
        currentTick++;
        for (int i = 0; i < stepsToCheck && currentFirePositions.Count > 0 && i + firePositionCurrentStep < currentFirePositions.Count; i++)
        {
            currentFirePositions[i + firePositionCurrentStep].Burn(burnRate, currentTick);
            if (currentFirePositions[i + firePositionCurrentStep].IsBurnedOut)
            {
                finishedBurningPoints.Add(currentFirePositions[i + firePositionCurrentStep]);
                SmolderFire(currentFirePositions[i + firePositionCurrentStep]);
                currentFirePositions.RemoveAt(i + firePositionCurrentStep);
            }
        }
        firePositionCurrentStep += stepsToCheck;
        if(firePositionCurrentStep >= currentFirePositions.Count)
        {
            firePositionCurrentStep = 0;
        }

        for (int i = 0; i < stepsToCheck && adjacentGridPoints.Count > 0 && i + adjacentPositionCurrentStep < adjacentGridPoints.Count; i++)
        {
            adjacentGridPoints[i + adjacentPositionCurrentStep].Burn(burnRate, currentTick);
            if (adjacentGridPoints[i + adjacentPositionCurrentStep].CanLightOnFire)
            {
                CreateNewFirePoint(adjacentGridPoints[i + adjacentPositionCurrentStep]);
                adjacentGridPoints.RemoveAt(i + adjacentPositionCurrentStep);
            }
        }
        adjacentPositionCurrentStep += stepsToCheck;
        if (adjacentPositionCurrentStep >= adjacentGridPoints.Count)
        {
            adjacentPositionCurrentStep = 0;
        }

        for (int i = 0; i < stepsToCheck && finishedBurningPoints.Count > 0 && i + SmolderCurrentStep < finishedBurningPoints.Count; i++)
        {
            finishedBurningPoints[i + SmolderCurrentStep].Smolder(burnRate, currentTick);
            if (finishedBurningPoints[i + SmolderCurrentStep].IsSmoldered)
            {
                RemoveRoomOnFire(finishedBurningPoints[i + SmolderCurrentStep]);
                finishedBurningPoints.RemoveAt(i + SmolderCurrentStep);
            }
        }
        SmolderCurrentStep += stepsToCheck;
        if (SmolderCurrentStep >= finishedBurningPoints.Count)
        {
            SmolderCurrentStep = 0;
        }
        yield return new WaitForEndOfFrame();
        isCalculatingFire = false;
    }

    private void CreateNewFirePoint(FireBlock point)
    {
        if(roomsOnFire[point.gridPoint.roomgrid.ID][TwoDToOneD(point.gridPoint.gridPosition, point.gridPoint.roomgrid)] <= FireState.ADJACENT )
        {
            point.isOnFire = true;
            currentFirePositions.Add(point);
            GridPoint[] adjacentRooms = point.gridPoint.GetAllAdjacentGridPoints();
            for (int i = 0; i < adjacentRooms.Length; i++)
            {
                if(roomsOnFire[adjacentRooms[i].roomgrid.ID][TwoDToOneD(adjacentRooms[i].gridPosition, adjacentRooms[i].roomgrid)] == FireState.NOTHING && adjacentRooms[i].isInside && 
                    Random.value < chanceToStartNewFire)
                {
                    adjacentGridPoints.Add(new FireBlock() { fireResistance = fireResistance + Random.Range(-variancefireResistance, variancefireResistance), currentOxygen = 10, currentFuel = fuel + Random.Range(-varianceFuel, varianceFuel), maxFuel = fuel, gridPoint = adjacentRooms[i], smolderTime = smolderTime + Random.Range(-varianceSmolder, varianceSmolder), lastActivatedTick = currentTick });
                }
            }
            roomsOnFire[point.gridPoint.roomgrid.ID][TwoDToOneD(point.gridPoint.gridPosition, point.gridPoint.roomgrid)] = FireState.BURNING;
        }
    }

    private void RemoveRoomOnFire(FireBlock block)
    {
        roomsOnFire[block.gridPoint.roomgrid.ID][TwoDToOneD(block.gridPoint.gridPosition, block.gridPoint.roomgrid)] = FireState.NOTHING;
    }
    private void SmolderFire(FireBlock block)
    {
        roomsOnFire[block.gridPoint.roomgrid.ID][TwoDToOneD(block.gridPoint.gridPosition, block.gridPoint.roomgrid)] = FireState.SMOLDERING;
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
            Gizmos.DrawSphere(point.gridPoint.center + Vector3.right * 0.3f, 0.3f);
        }
        Gizmos.color = Color.red;
        foreach (FireBlock point in currentFirePositions)
        {
            Gizmos.DrawSphere(point.gridPoint.center , 0.3f);
        }
        Gizmos.color = Color.green;
        foreach (FireBlock point in finishedBurningPoints)
        {
            Gizmos.DrawSphere(point.gridPoint.center + Vector3.forward * 0.3f, 0.3f);
        }
    }
}
