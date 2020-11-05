using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using Pieter.NavMesh;
using UnityEngine;

public class EntrancePoints : MonoBehaviour
{
    [SerializeField] private NavMeshGenerator generator = null;
    [SerializeField] private NavMeshEntrance[] entrancePoints = null;

    private void Awake()
    {
        UpdateEntranceValues();
    }

    public void UpdateEntranceValues()
    {
        UpdateVertexInfo();
        UpdateEntranceDoorDirections();
        UpdateConnectedTriangle();
    }

    private void UpdateVertexInfo()
    {
        for (int i = 0; i < entrancePoints.Length; i++)
        {
            entrancePoints[i].entranceMidPoint.ID = i;
            entrancePoints[i].entranceMidPoint.name = this.transform.parent.name + this.name + i;
            entrancePoints[i].entranceMidPoint.transform.SetSiblingIndex(i);
        }
    }

    public int Length
    {
        get { return entrancePoints.Length; }
    }

    public NavMeshEntrance[] Entrances
    {
        get { return entrancePoints; }
    }

    public NavMeshEntrance GetEntrance(int i)
    {
        return this.entrancePoints[i];
    }

    public void UpdateEntranceDoorDirections()
    {
        foreach (NavMeshEntrance entrance in entrancePoints)
        {
            entrance.generator = generator;
            entrance.CalculateDoorDirection();
            entrance.SetDoorController();
            
            //foreach (Transform child in entrance.entranceMidPoint)
            //{
            //    DestroyImmediate(child.gameObject);
            //}

            //Instantiate(doorPrefab, entrance.entranceMidPoint.position, entrance.entranceMidPoint.localRotation,
            //    entrance.entranceMidPoint);
        }
    }

    public void UpdateConnectedTriangle()
    {
        for (int i = 0; i < entrancePoints.Length; i++)
        {
            // Find the other point in the triangle by looking at all the other vertexes
            Vertex thirdPointid = FindThirdPointInTriangle(entrancePoints[i]);
            entrancePoints[i].connectedTriangle = generator.FindTriangle(entrancePoints[i].entrance1, entrancePoints[i].entrance2, thirdPointid);
        }
    }

    private Vertex FindThirdPointInTriangle(NavMeshEntrance entrance)
    {
        for (int j = 0; j < entrance.entrance1.Adjacent.Count; j++)
        {
            int adjID = entrance.entrance1.Adjacent[j].ID;
            for (int k = 0; k < entrance.entrance2.Adjacent.Count; k++)
            {
                if (adjID == entrance.entrance2.Adjacent[k].ID)
                {
                    return entrance.entrance2.Adjacent[k];
                }
            }
        }
        return null;
    }
}
