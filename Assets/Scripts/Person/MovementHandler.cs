using UnityEngine;
using Pieter.NavMesh;
using System.Collections.Generic;
using System;
using Pieter.GraphTraversal;
using Assets.Scripts;
using System.Linq;

public class MovementHandler : MonoBehaviour
{
    [SerializeField] private List<Vector3> targets = new List<Vector3>();
    [SerializeField] private NavMeshMovementLine[] pathArray = null;
    private RoomInformation currentRoom = null;
    [SerializeField] private GridMapHolder gridWorldHolder = null;
    private ITask activeTask = null;
    private Vector3 target;
    private Vertex associatedVertexTarget = null;
    [SerializeField] private float movementSpeed = 1;
    [SerializeField] private float rotationSpeed = 1;
    [SerializeField] private float minDistanceToTaskLocation = 0.3f;

    private int currentPathPosition = 0;
    public void SetPathIndexToZero() { currentPathPosition = 0; }

    public RoomInformation GetCurrentRoom => currentRoom;

    public List<NavMeshMovementLine> Path 
    { 
        get 
        {
            return pathArray.ToList(); 
        } 
        set 
        { 
            if (value != null) 
            {
                pathArray = value.ToArray();
            }
        } 
    }

    public bool IsValid => Path != null;

    public bool IsCurrentRoomOnFire { get { if (GetCurrentRoom != null) return GetCurrentRoom.IsOnFire; else return false; } }
    public bool IsCurrentGridPointOnFire 
    { 
        get
        {
            return GetCurrentRoom.roomGrid.GetGridPoint(this.transform.position).isOnFire;
        } 
    }

    private void Awake()
    {
        currentRoom = gridWorldHolder.GetRoomAtWorldPosition(this.transform.position);
    }

    public RoomInformation GetRoomInformationForLocation(Vector3 position)
    {
        return gridWorldHolder.GetRoomAtWorldPosition(position);
    }

    public bool SetPathFromPlayerTo(Vector3 posB, RoomInformation startingRoom,
                                                RoomInformation endingRoom, TraversalAStarNavigation traversalNav)
    {
        return SetPathFromTo(this.transform.position, posB, startingRoom, endingRoom, traversalNav);
    }

    public bool SetPathFromTo(Vector3 posA, Vector3 posB, RoomInformation startingRoom, 
                                                RoomInformation endingRoom, TraversalAStarNavigation traversalNav)
    {
        Path = null;
        if (startingRoom != null && endingRoom != null)
        {
            Path = LevelOfDetailNavigationSolver.GetLODPath(posA, posB, startingRoom, endingRoom, traversalNav, false);
            SetPathIndexToZero();
        }
        if (Path != null && Path.Count > 0)
        {
            return true;
        }
        else
        {
            Path = null;
            return false;
        }
    }

    public void ClearPath()
    {
        Path = new List<NavMeshMovementLine>();
    }

    public bool CheckPathIsEmpty()
    {
        return Path != null && Path.Count == 0 ;
    }

    public void SetActiveTask(ITask activeTask)
    {
        this.activeTask = activeTask;
    }

    public void SetPathToTargetToFirst()
    {
        targets.Add(Path[currentPathPosition].point);
        target = Path[currentPathPosition].point;
        associatedVertexTarget = Path[currentPathPosition].associatedVertex;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="taskUrgency"></param>
    /// <returns> -1 Cancel, 0 still active, 1 Done</returns>
    public int MoveToTarget(float taskUrgency)
    {
        int isDone = 0;
        //Check if the target is an open vertex
        if (associatedVertexTarget == null || (associatedVertexTarget != null && associatedVertexTarget.isPassable))
        {
            float distanceToTaskPosition = GetDistanceToPosition(target);
            if (distanceToTaskPosition >= minDistanceToTaskLocation)
            {
                WalkToTarget(taskUrgency);
                isDone = 0;
            }
            else
            {
                if (Path != null && Path.Count == currentPathPosition+1)
                {
                    this.transform.position = activeTask.GetInteractionPosition;
                    isDone = 1;
                }
                else if (Path != null && currentPathPosition+1 < Path.Count)
                {
                    if (associatedVertexTarget)
                    {
                        currentRoom = associatedVertexTarget.containedRoom;
                    }
                    else
                    {
                        currentRoom = gridWorldHolder.GetRoomAtWorldPosition(this.transform.position);
                    }
                    currentPathPosition++;
                    SetPathToTargetToFirst();
                    isDone = 0;
                }
                else if (Path == null)
                {
                    isDone = 1;
                }
            }
        }
        // The vertex is not passable, so we cancel the task
        else
        {
            ResetMovement();
            isDone = -1;
        }
        return isDone;
    }

    private void ResetMovement()
    {
        associatedVertexTarget = null;
        Path.Clear();
        SetPathIndexToZero();
    }

    /// <summary>
    /// Walk to the Target that is assigned to the current Task
    /// </summary>
    protected virtual void WalkToTarget(float urgencyLevel)
    {
        Vector3 direction = (target - this.transform.position).normalized;
        this.transform.rotation = Quaternion.Lerp(this.transform.rotation, Quaternion.LookRotation(direction, Vector3.up), urgencyLevel * rotationSpeed * Time.deltaTime);
    }

    private float GetDistanceToPosition(Vector3 target)
    {
        return Vector3.Distance(target, this.transform.position);
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawSphere(target, 0.7f);
        if (Path != null && Path.Count > 0)
        {
            Vector3 lastPosition = Path[0].point;
            for (int i = 0; i < Path.Count; i++)
            {
                if(i == currentPathPosition)
                {
                    Gizmos.color = Color.cyan;
                }
                else
                {
                    Gizmos.color = Color.blue;
                }
                Gizmos.DrawLine(lastPosition + Vector3.up, Path[i].point + Vector3.up);
                Gizmos.DrawWireCube(Path[i].point, Vector3.one * 0.4f);
                lastPosition = Path[i].point;
            }
        }
        if(targets != null && targets.Count > 0)
        {
            for (int i = targets.Count-1; i >= 0; i--)
            {
                Gizmos.color = new Color(0, 1, 0, 0.3f + ((float)i / (float)targets.Count));
                Gizmos.DrawCube(targets[i] + Vector3.up * 2, Vector3.one * (0.5f+ ((float)i / (float)targets.Count)));
            }
        }
    }
}