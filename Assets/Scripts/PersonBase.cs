using Assets.Scripts;
using Pieter.GraphTraversal;
using Pieter.NavMesh;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public enum ePersonState { Idle, GoTo, Working}
/// <summary>
/// A Generic person that uses a Task queue to get things done
/// </summary>
public class PersonBase : MonoBehaviour
{
    [SerializeField] private PersonAIDebugHolder debugHolder = null;
    // The persons current state 
    [SerializeField] private ePersonState startingState = ePersonState.Idle;
    private Stack<ePersonState> stateStack = new Stack<ePersonState>();
    private ePersonState currentState = ePersonState.Idle;

    // The tasks that he can perform
    private TaskListHolder personalTasks = null;
    [SerializeField] private TaskListHolder globalTasks = null;

    protected ITask currentTask = null;
    public string taskName = "";
    public string stateName = "";
    [SerializeField] private float minDistanceToTaskPosition = 0.1f;
    [SerializeField] private float movementSpeed = 1;

    [Space]
    private AnimationCommandController animatorController = null;

    [Space]
    private TraversalAStarNavigation traversalAStar = null;
    private TraversalGraphHolder traversalGraphHolder = null;
    private List<NavMeshMovementLine> path = new List<NavMeshMovementLine>();
    private Vector3 target;
    private Vertex associatedVertexTarget = null;

    [SerializeField] private LayerMask roomsMask = 0;
    private Collider[] hits = new Collider[10];

    protected RoomInformation currentRoom = null;
    private LevelGridGeneration levelGridGeneration = null;
    public bool isOnFire = false;
    public bool isCloseToFire = false;
    private bool hasCreatedFireTask = false;

    [SerializeField] private RoomGraphHolder roomGraph = null;

    private bool isWaitingForAnimationEnd = false;
    private Health health = null;
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
        health = GetComponent<Health>();
        if(health == null)
        {
            Debug.LogError("Could not find Health component", this);
        }
        debugHolder = ScriptableObject.CreateInstance<PersonAIDebugHolder>();
        levelGridGeneration = FindObjectOfType<LevelGridGeneration>();

        animatorController = GetComponent<AnimationCommandController>();
        traversalGraphHolder = FindObjectOfType<TraversalGraphHolder>();
        traversalAStar = new TraversalAStarNavigation(traversalGraphHolder);

        personalTasks = ScriptableObject.CreateInstance<TaskListHolder>();
        currentState = GoToState(startingState);

        currentRoom = GetRoomInformationForLocation(this.transform.position);
    }

    protected void CheckIfOnFire()
    {
        currentRoom = GetRoomInformationForLocation(this.transform.position);

        bool isFireClose = currentRoom.IsOnFire;
        if (isFireClose)
        {
            health.LoseHealth(burningDamageAmount);
        }

        for (int i = 0; !isFireClose && i < currentRoom.GetEntrances.Length; i++)
        {
            isFireClose = isFireClose || (currentRoom.GetEntrances[i].connectedEntrance != null && currentRoom.GetEntrances[i].IsPassable && currentRoom.GetConnectedRoomFromEntranceWithID(currentRoom.GetEntrances[i].ID).IsOnFire );
        }
        if (isFireClose)
        {
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
        }
    }

    private void CreateRunFromFireTask()
    {
        debugHolder.Log("Started creating fire task", eDebugImportance.Unimportant);
        RoomInformation selectedRoom = roomGraph.FindRoomFromStartMatching((x) => x >= 3, currentRoom.ID);
        if (selectedRoom != null)
        {
            hasCreatedFireTask = true;
            WalkTask walkTask = new WalkTask("Walk-RunFromFire", TaskScope.Personal, selectedRoom.GetRandomSpotInsideRoom, 10, 0, false, 2, () => { hasCreatedFireTask = false; debugHolder.Log("I am done running", eDebugImportance.Important); return true; });
            personalTasks.AddTask(walkTask);
            debugHolder.Log("Created Fire Task", eDebugImportance.Unimportant);
            isOnFire = false;
        }
        else
        {
            debugHolder.Log("Could not find a room to create the fire task", eDebugImportance.Error);
            WanderTask wanderTask = new WanderTask("Panic", TaskScope.Personal, currentRoom.GetRandomSpotInsideRoom, () => isOnFire, 10, 0, false, 2, 4, eAnimationType.Panic, CreatePanicTaskIfStillOnFire);
            AddNewTask(wanderTask);
            isOnFire = true;
        }
    }

    private bool CreatePanicTaskIfStillOnFire()
    {
        if (isOnFire)
        {
            debugHolder.Log("Create next wave of panic", eDebugImportance.Unimportant);
            WanderTask wanderTask = new WanderTask("Panic", TaskScope.Personal, currentRoom.GetRandomSpotInsideRoom, () => isOnFire, 10, 0, false, 2, 4, eAnimationType.Panic, CreatePanicTaskIfStillOnFire);
            AddNewTask(wanderTask);
            return true;
        }
        return false;
    }

    private ePersonState GoToState(ePersonState state)
    {
        if(stateStack.Count > 0)
        {
            CallExitForState(stateStack.Peek());
        }
        stateStack.Push(state);
        CallEntryForState(state);
        return state;
    }

    private void CallEntryForState(ePersonState state)
    {
        switch (state)
        {
            case ePersonState.Idle:
                IdleEntry();
                break;
            case ePersonState.GoTo:
                GoToEntry();
                break;
            case ePersonState.Working:
                WorkingEntry();
                break;
        }
    }

    private ePersonState FinishState()
    {
        CallExitForState(stateStack.Pop());
        if (stateStack.Count == 0)
        {
            stateStack.Push(ePersonState.Idle);
        }
        CallEntryForState(stateStack.Peek());
        return stateStack.Peek();
    }

    private void CallExitForState(ePersonState state)
    {
        switch (state)
        {
            case ePersonState.Idle:
                IdleExit();
                break;
            case ePersonState.GoTo:
                GoToExit();
                break;
            case ePersonState.Working:
                WorkingExit();
                break;
        }
    }

    protected void UpdateDebug()
    {
        if(currentTask != null)
        {
            taskName = currentTask.GetTaskInformation;
        }
        else
        {
            taskName = "Nothing";
        }
        stateName = currentState.ToString();
    }

    /// <summary>
    /// Work on the current task till it is finished
    /// </summary>
    public void WorkOnTask()
    {
        if (!currentTask.IsTaskValid)
        {
            debugHolder.Log("Task is invalid (" + currentTask.GetTaskInformation + ")", eDebugImportance.Error);
            currentTask = null;
        }

        switch (currentState)
        {
            case ePersonState.Idle:
                IdleState();
                break;
            case ePersonState.GoTo:
                WalkingState();
                break;
            case ePersonState.Working:
                WorkingState();
                break;
        }
    }

    private void IdleEntry()
    {
        debugHolder.Log("Idle Entry Called", eDebugImportance.Entry);
        animatorController.ChangeState(eAnimationType.Idle);
    }
    /// <summary>
    /// The Idle state checks the transitions to go to the next states
    /// If the person is too far from the position, change states,
    /// If the person is at the position, change states
    /// </summary>
    private void IdleState()
    {
        if (currentTask == null || !currentTask.IsTaskValid)
        {
            return;
        }
        if (isCloseToFire && currentTask.Isinterruptible)
        {
            ReturnSelectedTaskToTaskList();
            return;
        }
        float distanceToTaskPosition = GetDistanceToTaskPosition(currentTask);
        if (distanceToTaskPosition >= minDistanceToTaskPosition)
        {
            currentState = GoToState(ePersonState.GoTo);
        }
        else
        {
            currentState = GoToState(ePersonState.Working);
        }
    }
    private void IdleExit()
    {
        debugHolder.Log("Idle Exit Called", eDebugImportance.Exit);
    }


    private void GoToEntry()
    {
        debugHolder.Log("GOTO Entry Called", eDebugImportance.Entry);
        RoomInformation startingRoom = null;
        startingRoom = currentRoom;
        RoomInformation endingRoom = null;
        endingRoom = GetRoomInformationForLocation(currentTask.GetInteractionPosition);
        path = null;
        if (startingRoom != null && endingRoom != null)
        {
            path = LevelOfDetailNavigationSolver.GetLODPath(this.transform.position, currentTask.GetInteractionPosition, startingRoom, endingRoom, traversalAStar);
            debugHolder.Log("Found path of length " + path.Count, eDebugImportance.Unimportant);
        }
        if (path != null && path.Count > 0)
        {
            animatorController.ChangeState(eAnimationType.Walk);
            animatorController.SetFloat("SpeedMultiplier", currentTask.GetTaskUrgencyLevel);
            target = path[0].point;
            associatedVertexTarget = path[0].associatedVertex;
            path.RemoveAt(0);
        }
    }
    /// <summary>
    /// While the person is in the GoToState, check the distance,
    /// if the person is close, pop the state
    /// if the person is far away, walk to the position
    /// </summary>
    private void WalkingState()
    {
        if(currentTask == null)
        {
            return;
        }
        if (isCloseToFire && currentTask.Isinterruptible)
        {
            ReturnSelectedTaskToTaskList();
            currentState = FinishState();
        }
        // No path was found to the target task
        if (path == null)
        {
            ReturnSelectedTaskToTaskList();

            currentState = FinishState();
        }
        else
        {
            //Check if the target is a open vertex
            if(associatedVertexTarget == null || ( associatedVertexTarget != null && associatedVertexTarget.isPassable))
            {
                float distanceToTaskPosition = GetDistanceToPosition(target);
                if (distanceToTaskPosition >= minDistanceToTaskPosition)
                {
                    WalkToTarget();
                }
                else
                {
                    if (path != null && path.Count == 0)
                    {
                        this.transform.position = currentTask.GetInteractionPosition;
                        currentState = FinishState();
                    }
                    else if (path != null)
                    {
                        target = path[0].point;
                        associatedVertexTarget = path[0].associatedVertex;
                        path.RemoveAt(0);
                    }
                    else if (path == null)
                    {
                        currentState = FinishState();
                    }
                }
            }
            // The vertex is not passable, so we cancel the task
            else
            {
                associatedVertexTarget = null;
                path.Clear();
                ReturnSelectedTaskToTaskList();
                currentState = FinishState();
            }
        }
    }

    private void ReturnSelectedTaskToTaskList()
    {
        currentTask.GetTaskInformation = currentTask.GetTaskInformation + " Returned";
        switch (currentTask.GetTaskScope)
        {
            case TaskScope.Global:
                globalTasks.AddTask(currentTask);
                break;
            case TaskScope.Personal:
                personalTasks.AddTask(currentTask);
                break;
        }
        currentTask = null;
    }

    private void GoToExit()
    {
        debugHolder.Log("GOTO Exit Called", eDebugImportance.Exit);
    }

    private void WorkingEntry()
    {
        debugHolder.Log("Working Entry Called", eDebugImportance.Entry);
        if (currentTask.DoesWork)
        {
            isWaitingForAnimationEnd = true;
            animatorController.ChangeState(currentTask.GetWorkAnimationType);
            currentTask.SetWorkTimer(currentTask.GetWorkTime);
            this.transform.position = currentTask.GetInteractionPosition;
            this.transform.rotation = currentTask.GetInteractionRotation;
        }
    }

    /// <summary>
    /// While the person is in the working state, Check if the work is done
    /// pop back to idle. Otherwise, keep working at the problem
    /// </summary>
    private void WorkingState()
    {
        if(currentTask == null)
        {
            return;
        }

        if (!currentTask.DoesWork)
        {
            currentTask.GetWorkDoneFunction?.Invoke();
            currentState = FinishState();
            return;
        }
        if(isCloseToFire && currentTask.Isinterruptible)
        {
            animatorController.ChangeState(currentTask.GetWorkAnimationType);
            ReturnSelectedTaskToTaskList();

            currentState = FinishState();
            return;
        }

        currentTask.UpdateWorkTimer(Time.deltaTime);
        if (currentTask.IsWorkDone)
        {
            currentTask.GetWorkDoneFunction?.Invoke();
            animatorController.ChangeState(currentTask.GetWorkAnimationType);
        }
    }

    public void WorkingAnimationDone()
    {
        if (isWaitingForAnimationEnd && currentTask != null && currentTask.IsWorkDone)
        {
            debugHolder.Log("Waiting for animation finished", eDebugImportance.Unimportant);
            isWaitingForAnimationEnd = false;
            currentState = FinishState();
        }
    }
    private void WorkingExit()
    {
        debugHolder.Log("Working Exit Called", eDebugImportance.Exit);
        if (currentTask != null)
        {
            currentTask = currentTask.GetRandomFollowUpTask();
        }
    }

    /// <summary>
    /// Walk to the Target that is assigned to the current Task
    /// </summary>
    protected virtual void WalkToTarget()
    {
        if(currentTask != null)
        {
            Vector3 direction = (target - this.transform.position).normalized;
            this.transform.rotation = Quaternion.Lerp(this.transform.rotation, Quaternion.LookRotation(direction, Vector3.up), currentTask.GetTaskUrgencyLevel * movementSpeed * Time.deltaTime);
        }
    }


    /// <summary>
    /// Get a new task from the list of tasks
    /// </summary>
    public ITask GetNewTask()
    {
        debugHolder.Log("Get a new Task", eDebugImportance.Unimportant);
        List<ITask> allTasks = new List<ITask>();
        allTasks.AddRange(personalTasks.Tasks);
        allTasks.AddRange(globalTasks.Tasks);
        List<NavMeshMovementLine> newPath = new List<NavMeshMovementLine>();
        for (int i = 0; i < allTasks.Count; i++)
        {
            if (!allTasks[i].IsTaskValid)
            {
                switch (allTasks[i].GetTaskScope)
                {
                    case TaskScope.Global:
                        globalTasks.Remove(allTasks[i]);
                        break;
                    case TaskScope.Personal:
                        personalTasks.Remove(allTasks[i]);
                        break;
                }
                continue;
            }
            newPath.Clear();
            //Quick check to see if the task is possible
            if (allTasks[i] != null)
            {
                RoomInformation targetRoom = GetRoomInformationForLocation(allTasks[i].GetInteractionPosition);
                if (targetRoom != null)
                {
                    Vertex targetVertex = targetRoom.GetCenterVertex;
                    newPath = traversalAStar.GetPathFromTo(currentRoom.GetCenterVertex, targetVertex);
                }
            }
            else
            {
                Debug.Log("Could not find route");
                newPath = null;
            }
            if (currentRoom != null && newPath != null && newPath.Count > 0)
            {
                switch (allTasks[i].GetTaskScope)
                {
                    case TaskScope.Global:
                        globalTasks.Remove(allTasks[i]);
                        break;
                    case TaskScope.Personal:
                        personalTasks.Remove(allTasks[i]);
                        break;
                }
                debugHolder.Log("Found a task (" + allTasks[i].GetTaskInformation + ")", eDebugImportance.Unimportant);
                newPath.Clear();
                return allTasks[i];
            }
        }
        return new WanderTask("Wander "+ currentRoom.name, TaskScope.Personal, currentRoom.GetRandomSpotInsideRoom, null, 2, 5, true, 1, 3);
    }

    /// <summary>
    /// Get a new task from the list of follow up tasks from the current task
    /// </summary>
    public ITask GetNewFollowUpTask()
    {
        if (currentTask.FollowUpTasks.Count > 0)
        {
            float highestPriority = currentTask.FollowUpTasks[0].GetPriority;
            ITask selectedTask = currentTask.FollowUpTasks[0];
            foreach (ITask newTask in currentTask.FollowUpTasks)
            {
                if (highestPriority < newTask.GetPriority)
                {
                    highestPriority = newTask.GetPriority;
                    selectedTask = newTask;
                }
            }

            return selectedTask;
        }
        else
        {
            return null;
        }
    }

    public void AddNewTask(ITask task)
    {
        personalTasks.AddTask(task);
    }

    private float GetDistanceToTaskPosition(ITask targetTask)
    {
        if (targetTask != null && targetTask.IsTaskValid)
        {
            return Vector3.Distance(targetTask.GetInteractionPosition, this.transform.position);
        }
        else
        {
            currentTask = null;
            return 0;
        }
    }
    private float GetDistanceToPosition(Vector3 target)
    {
        return Vector3.Distance(target, this.transform.position);
    }

    public void FillTiredness(float fillAmount)
    {
        Debug.Log(fillAmount);
    }

    private RoomInformation GetRoomInformationForLocation(Vector3 position)
    {
        return levelGridGeneration.GetRoomAtWorldPosition(position); 
    }

    protected void PersonBaseOnDrawGizmos()
    {
        Gizmos.DrawSphere(target, 0.7f);
        Vector3 lastPosition = this.transform.position;
        if(path != null)
        {
            foreach (NavMeshMovementLine line in path)
            {
                Gizmos.color = Color.cyan;
                Gizmos.DrawLine(lastPosition + Vector3.up, line.point + Vector3.up);
                Gizmos.DrawWireCube(line.point, Vector3.one * 0.4f);
                lastPosition = line.point;
            }
        }
    }
}
