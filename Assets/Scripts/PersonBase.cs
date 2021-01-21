using Assets.Scripts;
using Assets.Scripts.Person;
using Pieter.GraphTraversal;
using Pieter.NavMesh;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public enum ePersonState { Idle, Move, Work}
/// <summary>
/// A Generic person that uses a Task queue to get things done
/// </summary>
public class PersonBase : MonoBehaviour
{
    protected PersonTaskHandler taskHandler = null;
    protected StateHandler stateHandler = null;
    [SerializeField] protected MovementHandler movementHandler = null;
    [SerializeField] protected PersonAIDebugHolder debugHolder = null;

    // The tasks that he can perform
    [SerializeField] private TaskListHolder globalTasks = null;
    private ePersonState startingState = ePersonState.Idle;
    private StateBlock idleState = null;
    private StateBlock moveState = null;
    private StateBlock workState = null;

    public string taskName = "";
    public string stateName = "";
    [SerializeField] private float minDistanceToTaskPosition = 0.1f;

    [Space]
    protected AnimationCommandController animatorController = null;

    [Space]
    protected TraversalAStarNavigation traversalAStar = null;
    private TraversalGraphHolder traversalGraphHolder = null;

    public bool isOnFire = false;
    public bool isCloseToFire = false;
    private bool hasCreatedFireTask = false;

    [SerializeField] private RoomGraphHolder roomGraph = null;

    [SerializeField] private float burningDamageAmount = 0.1f;
    [SerializeField] private Health healthController = null;


    public bool CheckIfAlive()
    {
        if(healthController == null)
        {
            Debug.LogError("Health controller is null");
        }
        return healthController.IsAlive;
    }

    public void StartUp()
    {
        debugHolder = ScriptableObject.CreateInstance<PersonAIDebugHolder>();
        animatorController = GetComponent<AnimationCommandController>();
        traversalGraphHolder = FindObjectOfType<TraversalGraphHolder>();
        traversalAStar = new TraversalAStarNavigation(traversalGraphHolder);
        taskHandler = new PersonTaskHandler(globalTasks, ScriptableObject.CreateInstance<TaskListHolder>(), roomGraph);

        stateHandler = new StateHandler(null);
        idleState = new IdleStateBlock(debugHolder, animatorController, taskHandler, stateHandler, minDistanceToTaskPosition, this.transform);
        AddState(ePersonState.Idle, idleState);
        moveState = new MoveStateBlock(debugHolder, animatorController, taskHandler, traversalAStar, movementHandler);
        AddState(ePersonState.Move, moveState);
        workState = new WorkStateBlock(debugHolder, animatorController, taskHandler);
        AddState(ePersonState.Work, workState);
        stateHandler.TransitionToState(startingState);

        if (healthController == null)
        {
            Debug.LogError("Could not find Health component", this);
        }
        else
        {
            healthController.OnDeath += OnDeath;
        }

        void AddState(ePersonState state, StateBlock stateBlock)
        {
            stateHandler.AddState(state, stateBlock.Entry, stateBlock.Exit, stateBlock.Update);
        }
    }

    protected virtual void OnDeath()
    {

    }

    protected void CheckIfOnFire()
    {
        if (movementHandler.GetCurrentRoom == null)
        {
            return;
        }
        bool isFireClose = movementHandler.IsCurrentRoomOnFire;
        bool isOnFirePoint= movementHandler.IsCurrentGridPointOnFire;
        if (isOnFirePoint)
        {
            healthController.LoseHealth(burningDamageAmount);
        }

        for (int i = 0; !isFireClose && i < movementHandler.GetCurrentRoom.GetEntrances.Length; i++)
        {
            isFireClose = isFireClose || (movementHandler.GetCurrentRoom.GetEntrances[i].connectedEntrance != null &&
                                            movementHandler.GetCurrentRoom.GetEntrances[i].IsPassable && 
                                            movementHandler.GetCurrentRoom.GetConnectedRoomFromEntranceWithID(movementHandler.GetCurrentRoom.GetEntrances[i].ID).IsOnFire );
        }
        if (isFireClose)
        {
            if (taskHandler.IsActiveTaskInterruptible)
            {
                taskHandler.InteruptActiveTask();
            }
            if (!hasCreatedFireTask)
            {
                CreateRunFromFireTask();
            }
            isCloseToFire = true;
        }
        else
        {
            isCloseToFire = false;
            isOnFire = false;
            isOnFirePoint = false;
        }
    }

    private void CreateRunFromFireTask()
    {
        debugHolder.Log("Started creating fire task", eDebugImportance.Unimportant);
        RoomInformation selectedRoom = roomGraph.FindRoomFromStartMatching((x) => x >= 3, movementHandler.GetCurrentRoom.ID);
        if (selectedRoom != null)
        {
            hasCreatedFireTask = true;
            WalkTask walkTask = new WalkTask("Walk-RunFromFire", TaskScope.Personal, selectedRoom.GetRandomSpotInsideRoom, selectedRoom, 10, 0, false, 2, () => { hasCreatedFireTask = false; debugHolder.Log("I am done running", eDebugImportance.Important); return true; });
            taskHandler.AddNewTask(walkTask);
            
            debugHolder.Log("Created Fire Task", eDebugImportance.Unimportant);
            isOnFire = false;
        }
        else
        {
            debugHolder.Log("Could not find a room to create the fire task", eDebugImportance.Error);
            WanderTask wanderTask = new WanderTask("Panic", TaskScope.Personal, movementHandler.GetCurrentRoom.GetRandomSpotInsideRoom, movementHandler.GetCurrentRoom, () => isOnFire, 10, 0, false, 2, 4, eAnimationType.Panic, CreatePanicTaskIfStillOnFire);
            taskHandler.AddNewTask(wanderTask);
            isOnFire = true;
        }
    }

    private bool CreatePanicTaskIfStillOnFire()
    {
        if (isOnFire)
        {
            debugHolder.Log("Create next wave of panic", eDebugImportance.Unimportant);
            WanderTask wanderTask = new WanderTask("Panic", TaskScope.Personal, movementHandler.GetCurrentRoom.GetRandomSpotInsideRoom, movementHandler.GetCurrentRoom, () => isOnFire, 10, 0, false, 2, 4, eAnimationType.Panic, CreatePanicTaskIfStillOnFire);
            taskHandler.AddNewTask(wanderTask);
            return true;
        }
        return false;
    }

    protected void UpdateDebug()
    {
        taskName = taskHandler.GetTaskName;
        stateName = stateHandler.GetCurrentStateName;
    }

    /// <summary>
    /// Work on the current task till it is finished
    /// </summary>
    public void WorkOnTask()
    {
        if (!taskHandler.IsActiveTaskValid)
        {
            debugHolder.Log("Task is invalid (" + taskHandler.GetTaskName + ")", eDebugImportance.Error);
        }

        stateHandler.Update();
    }


    private bool CloseToFireCancel()
    {
        if (isCloseToFire && taskHandler.IsActiveTaskInterruptible)
        {
            taskHandler.ReturnActiveTask();

            stateHandler.FinishState();
            return true;
        }
        return false;
    }

    protected void PersonBaseOnDrawGizmos()
    {
        if(movementHandler != null && movementHandler.GetCurrentRoom != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(movementHandler.GetCurrentRoom.transform.position + movementHandler.GetCurrentRoom.center, 1);

        }
    }
}
