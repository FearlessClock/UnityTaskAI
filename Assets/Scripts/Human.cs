
using Pieter.NavMesh;
using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class Human : PersonBase
{
    // We suppose that the start position is where it spawns
    [SerializeField] private Vector3 startPosition;
    [SerializeField] private RoomInformation startRoom = null;
    public Action<Human> OnScientistInactive = null;
    [SerializeField] private float wanderChangeRoomChance = 50;

    private void OnDestroy()
    {
        taskHandler.DestroyTask();    
    }

    private void Awake()
    {
        StartUp();
        startRoom = movementHandler.GetRoomInformationForLocation(startPosition);
        DayCycleController.OnEndOfDay += OnEndOfDay;
    }

    private void OnEndOfDay()
    {
        InteruptTask();
        WalkTask task = new WalkTask("Leave the building", TaskScope.Personal, startPosition, startRoom, -1, -1, true, 1, OnLeftBuilding);
        debugHolder.Log("End of Day, return home", eDebugImportance.Unimportant);
        taskHandler.AddNewTask(task);
    }

    private bool OnLeftBuilding()
    {
        OnScientistInactive?.Invoke(this);
        return true;
    }

    internal void SetStartPoint(Vector3 position)
    {
        startPosition = position;
    }

    private void OnEnable()
    {
        ScientistCounterController.instance.AddScientist();
    }

    protected override void OnDeath()
    {
        ScientistCounterController.instance.KillScientist();
        DayCycleController.OnEndOfDay -= OnEndOfDay;
    }

    private void OnDisable()
    {
        DayCycleController.OnEndOfDay -= OnEndOfDay;
    }

    private void Update()
    {
        if (!CheckIfAlive())
        {
            return;
        }
        CheckIfOnFire();
        if (taskHandler.IsActiveTaskValid)
        {
            WorkOnTask();
        }
        else
        {
            if (!taskHandler.SetNewActiveTask(movementHandler.GetCurrentRoom.TraversalGenerator, this.transform.position, traversalAStar))
            {
                float randomValue = Random.Range(0, 100);
                RoomInformation room = movementHandler.GetCurrentRoom;
                if (randomValue < wanderChangeRoomChance)
                {
                    NavMeshEntrance randomEntrance = room.GetRandomEntrance();
                    if (randomEntrance.IsUsed && randomEntrance.IsPassable)
                    {
                        room = room.GetConnectedRoomFromEntranceWithID(randomEntrance.ID);
                    }
                }
                WanderTask task = new WanderTask("Wander " + room.name, TaskScope.Personal, room.GetRandomSpotInsideRoom, room, null, 2, 5, true, 1, 3);
                debugHolder.Log("No active and available tasks found, setting wander as active to " + room.name, eDebugImportance.Unimportant);
                taskHandler.SetActiveTask(task);
            }

        }

        UpdateDebug();
    }

    public void AddNewTask(ITask basicTask)
    {
        taskHandler.AddNewTask(basicTask);
    }

    private void OnDrawGizmos()
    {
        if(taskHandler != null && taskHandler.IsActiveTaskValid)
        {
            Gizmos.color = Color.red;
            
            Gizmos.DrawWireSphere(taskHandler.ActiveTask.GetInteractionPosition, 0.2f);
        }

        base.PersonBaseOnDrawGizmos();
    }
}