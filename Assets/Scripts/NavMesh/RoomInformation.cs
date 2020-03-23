using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pieter.NavMesh;
using System;
using Pieter.GraphTraversal;
using UnityEditorInternal;

public class RoomInformation : MonoBehaviour
{
    private int roomID = -1;

    public void SetID(int id)
    {
        roomID = id;
    }
    [SerializeField] private NavMeshGenerator meshGenerator = null;
    public NavMeshGenerator NavMeshGenerator => meshGenerator;
    [SerializeField] private Pieter.GraphTraversal.TraversalGenerator traversalGenerator = null;

    [SerializeField] private NavMeshHolder navMeshHolder = null;
    private AStarNavMeshNavigation navMeshNavigation = null;
    public AStarNavMeshNavigation NavMeshNavigation => navMeshNavigation;

    public bool showOccupiedSpace = false;
    public Vector3 center;
    public Vector3 extents;

    private void Awake()
    {
        navMeshHolder.AddNavMesh(meshGenerator);
        navMeshNavigation = new AStarNavMeshNavigation(navMeshHolder);
    }

    private void OnValidate()
    {
        if (meshGenerator != null)
        {
            meshGenerator.containedRoom = this;
        }

        if (traversalGenerator != null)
        {
            traversalGenerator.containedRoom = this;
        }

        navMeshHolder.ResetData();
        navMeshHolder.AddNavMesh(meshGenerator);
    }

    public override bool Equals(object other)
    {
        if (other == null)
        {
            return false;
        }

        RoomInformation room = (RoomInformation) other;
        return room.roomID == roomID;
    }

    private void Start()
    {
        meshGenerator.UpdateEntranceDoorDirections();
    }

    public Pieter.GraphTraversal.TraversalGenerator TraversalGenerator => traversalGenerator;

    public NavMeshEntrance GetRandomEntrance()
    {
        return meshGenerator.GetRanomEntrance;
    }
    public NavMeshEntrance GetEntrance(int index)
    {
        return meshGenerator.GetEntrance(index);
    }

    public NavMeshEntrance[] GetAllEntrances()
    {
        return meshGenerator.Entrances;
    }

    private void OnDrawGizmos()
    {
        if (showOccupiedSpace)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawCube(this.transform.position + center, extents) ;
        }
    }
}
