using System;
using System.Collections;
using System.Collections.Generic;
using Pieter.Grid;
using Pieter.NavMesh;
using UnityEngine;
using Random = UnityEngine.Random;

public class FireController : MonoBehaviour
{
    [SerializeField] private RoomInformation room = null;
    [SerializeField] private GameObject firePrefab = null;
    private List<GridPoint> currentFirePositions = new List<GridPoint>();
    private List<GridPoint> adjacentGridPoints = new List<GridPoint>();

    [SerializeField] private FireGenerator fireGenerator = null;

    private bool fireIsSpreading = false;
    [SerializeField] private float waitTime = 2;
    private float waitTimer = 0;

    public void StartFire()
    {
        currentFirePositions.Clear();
        adjacentGridPoints.Clear();
        fireGenerator.StartFire(room, 0);
        //GridPoint startpoint = room.roomGrid.GetRandomGridPoint();
        //Instantiate(firePrefab, startpoint.center, room.transform.rotation, this.transform);
        //currentFirePositions.Add(startpoint);
        //adjacentGridPoints.AddRange(startpoint.GetAllAdjacentGridPoints());
        //fireIsSpreading = true;
    }

    private void Update()
    {
        if (!fireIsSpreading)
        {
            return;
        }

        waitTimer -= Time.deltaTime;
        if (waitTimer < 0 && adjacentGridPoints.Count > 0)
        {
            int index = Random.Range(0, adjacentGridPoints.Count);
            AddFire(adjacentGridPoints[index]);
            adjacentGridPoints.RemoveAt(index);
            waitTimer = waitTime;
        }
    }

    private void AddFire(GridPoint point)
    {
        point.isOnFire = true;
        Instantiate(firePrefab, point.center, room.transform.rotation, this.transform);
        currentFirePositions.Add(point);
        foreach (GridPoint gridPoint in point.GetAllAdjacentGridPoints())
        {
            if (!adjacentGridPoints.Contains(gridPoint) && !gridPoint.isOnFire)
            {
                adjacentGridPoints.Add(gridPoint);
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        
        foreach (GridPoint point in adjacentGridPoints)
        {
            Gizmos.DrawSphere(point.center, 0.3f);
        }
    }
}
