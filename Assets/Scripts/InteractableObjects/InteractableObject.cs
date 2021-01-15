using Pieter.GraphTraversal;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Build;
using UnityEngine;

public class InteractableObject : MonoBehaviour
{
    [SerializeField] protected Transform interactionPoint = null;
    [SerializeField] protected RoomGraphHolder roomGraphHolder = null;
    private bool isWorking = true;
    public bool IsWorking => isWorking;

    public RoomInformation GetContainedRoom()
    {
        return roomGraphHolder.FindRoomAtLocation(interactionPoint.position);
    }

    public virtual BasicTask GenerateTask()
    {
        return new BasicTask("Interactable-" + this.name, TaskScope.Global, interactionPoint, GetContainedRoom(), 10, 10, 10, true, 1, null, eAnimationType.Work, this);
    }

    public virtual bool Work() { return true; }

    public void ExplodeObject()
    {
        isWorking = false;
    }
}
