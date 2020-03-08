using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pieter.NavMesh;
using System;

public class RoomInformation : MonoBehaviour
{
    [SerializeField] private NavMeshGenerator meshGenerator = null;

    public NavMeshGenerator NavMeshGenerator
    {
        get { return meshGenerator; }
    }

    public NavMeshEntrance GetRandomEntrance()
    {
        return meshGenerator.GetRanomEntrance;
    }
    public NavMeshEntrance GetEntrance(int index)
    {
        return meshGenerator.GetEntrance(index);
    }
}
