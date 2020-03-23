using System;
using System.Collections;
using System.Collections.Generic;
using Assets.Scripts;
using Pieter.GraphTraversal;
using UnityEngine;
using Pieter.NavMesh;

public enum ePersonState { Idle, GoTo, Working}
/// <summary>
/// A Generic person that uses a Task queue to get things done
/// </summary>
public class PersonBase : MonoBehaviour
{
    // The persons current state 
    [SerializeField] private ePersonState startingState = ePersonState.Idle;
    private Stack<ePersonState> stateStack = new Stack<ePersonState>();
    private ePersonState currentState = ePersonState.Idle;

    // The tasks that he can perform
    private TaskListHolder personalTasks = null;
    [SerializeField] private TaskListHolder globalTasks = null;

    [SerializeReference] protected TaskBase currentTask = null;

    [SerializeField] private float minDistanceToTaskPosition = 0.1f;
    [SerializeField] private float movementSpeed = 1;

    [Space]
    private Animator animator = null;

    [Space]
    private TraversalAStarNavigation traversalAStar = null;
    private TraversalGraphHolder traversalGraphHolder = null;
    private List<NavMeshMovementLine> path = new List<NavMeshMovementLine>();
    private Vector3 target;

    [SerializeField] private LayerMask roomsMask = 0;
    private Collider[] hits = new Collider[10];

    public void StartUp()
    {
        animator = GetComponent<Animator>();
        traversalGraphHolder = FindObjectOfType<TraversalGraphHolder>();
        traversalAStar = new TraversalAStarNavigation(traversalGraphHolder);

        personalTasks = ScriptableObject.CreateInstance<TaskListHolder>();
        currentState = GoToState(startingState);
        animator.SetBool(currentState.ToString(), true);
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

    /// <summary>
    /// Work on the current task till it is finished
    /// </summary>
    public void WorkOnTask()
    {
        switch (currentState)
        {
            case ePersonState.Idle:
                IdleState();
                break;
            case ePersonState.GoTo:
                GoToState();
                break;
            case ePersonState.Working:
                WorkingState();
                break;
        }
    }

    private void IdleEntry()
    {
        animator.SetBool(ePersonState.Idle.ToString(), true);
    }
    /// <summary>
    /// The Idle state checks the transitions to go to the next states
    /// If the person is too far from the position, change states,
    /// If the person is at the position, change states
    /// </summary>
    private void IdleState()
    {
        if (currentTask == null)
        {
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
        animator.SetBool(ePersonState.Idle.ToString(), false);
    }


    private void GoToEntry()
    {
        animator.SetBool(ePersonState.GoTo.ToString(), true);

        RoomInformation startingRoom = null;
        startingRoom = GetRoomInformationForLocation(this.transform.position);
        RoomInformation endingRoom = null;
        endingRoom = GetRoomInformationForLocation(currentTask.interactionPoint.position);
        if (startingRoom != null && endingRoom != null)
        {
            path = LevelOfDetailNavigationSolver.GetLODPath(this.transform.position, currentTask.interactionPoint.position, startingRoom, endingRoom, traversalAStar);
        }
        if (path != null && path.Count > 0)
        {
            target = path[0].point;
            path.RemoveAt(0);
        }
        else
        {
            currentState = FinishState();
        }
    }
    /// <summary>
    /// While the person is in the GoToState, check the distance,
    /// if the person is close, pop the state
    /// if the person is far away, walk to the position
    /// </summary>
    private void GoToState()
    {
        float distanceToTaskPosition = GetDistanceToPosition(target);
        if (distanceToTaskPosition >= minDistanceToTaskPosition)
        {
            WalkToTarget();
        }
        else
        {
            if(path != null && path.Count == 0)
            {
                currentState = FinishState();
            }
            else if(path != null)
            {
                target = path[0].point;
                path.RemoveAt(0);
            }
            else if(path == null)
            {
                currentState = FinishState();
            }
        }
    }

    private void GoToExit()
    {
        animator.SetBool(ePersonState.GoTo.ToString(), false);
    }

    private void WorkingEntry()
    {
        animator.SetBool(currentTask.workAnimationType.ToString(), true);
        currentTask.workTimer = currentTask.workTime;
        this.transform.position = currentTask.interactionPoint.position;
        this.transform.rotation = currentTask.interactionPoint.rotation;
    }

    /// <summary>
    /// While the person is in the working state, Check if the work is done
    /// pop back to idle. Otherwise, keep working at the problem
    /// </summary>
    private void WorkingState()
    {
        currentTask.workTimer -= Time.deltaTime;
        if (currentTask.IsWorkDone())
        {
            animator.SetBool(currentTask.workAnimationType.ToString(), false);
        }
    }

    public void WorkingAnimationDone()
    {
        currentState = FinishState();
    }
    private void WorkingExit()
    {
        currentTask = currentTask.GetRandomFollowUpTask();
    }

    /// <summary>
    /// Walk to the Target that is assigned to the current Task
    /// </summary>
    protected virtual void WalkToTarget()
    {
        Vector3 direction = ( target - this.transform.position).normalized;
        this.transform.rotation = Quaternion.Lerp(this.transform.rotation, Quaternion.LookRotation(direction, Vector3.up), movementSpeed * Time.deltaTime); ;
    }


    /// <summary>
    /// Get a new task from the list of tasks
    /// </summary>
    public TaskBase GetNewTask()
    {
        TaskBase personalTaskBase = personalTasks.PeekHighestPriorityTask();
        TaskBase globalTaskBase = globalTasks.PeekHighestPriorityTask();
        if(personalTaskBase == null)
        {
            return globalTasks.GetHighestPriorityTask();
        }
        if(globalTaskBase == null)
        {
            return personalTasks.GetHighestPriorityTask();
        }
        if(personalTaskBase.priority <= globalTaskBase.priority)
        {
            return personalTasks.GetHighestPriorityTask();
        }
        else
        {
            return globalTasks.GetHighestPriorityTask();
        }
    }

    /// <summary>
    /// Get a new task from the list of follow up tasks from the current task
    /// </summary>
    public TaskBase GetNewFollowUpTask()
    {
        if (currentTask.FollowUpTasks.Count > 0)
        {
            float highestPriority = currentTask.followUpTasks[0].priority;
            TaskBase selectedTask = currentTask.followUpTasks[0];
            foreach (TaskBase newTask in currentTask.followUpTasks)
            {
                if (highestPriority < newTask.priority)
                {
                    highestPriority = newTask.priority;
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

    public void AddNewTask(TaskBase task)
    {
        personalTasks.AddTask(task);
    }

    private float GetDistanceToTaskPosition(TaskBase targetTask)
    {
        return Vector3.Distance(targetTask.interactionPoint.position, this.transform.position);
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
        RoomInformation startingRoom = null;
        int nmbrOfHits = Physics.OverlapSphereNonAlloc(position, 1, hits, roomsMask);
        if (nmbrOfHits > 0)
        {
            for (int i = 0; i < nmbrOfHits; i++)
            {
                startingRoom = hits[i].GetComponent<RoomInformation>();
                if (startingRoom != null)
                {
                    break;
                }
            }
        }

        return startingRoom;
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawSphere(target, 0.7f);
        Vector3 lastPosition = this.transform.position;
        foreach (NavMeshMovementLine line in path)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawLine(lastPosition +Vector3.up, line.point+Vector3.up);
            Gizmos.DrawWireCube(line.point, Vector3.one * 0.4f);
            lastPosition = line.point;
        }
    }
}
