using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

[System.Serializable]
public class WalkTask : ITask
{
    private TaskScope taskScope = TaskScope.Global;
    private Vector3 target;
    private RoomInformation containedRoom = null;

    private eAnimationType workAnimationType = eAnimationType.Idle;
    private float workTime = 0;
    private float workTimer = 0;
    private float timeLimit = 0;
    private float timeLimitTimer = 0;
    private float priority = 0;
    private List<ITask> followUpTasks = new List<ITask>();
    private bool isinterruptible = true;
    private bool isValid = true;
    private int urgencyLevel = 1;
    private Func<bool> onDoneFunction = null;
    private Action OnTaskInvalidate = null;
    public string taskName = "";

    public WalkTask(string name, TaskScope scope, Vector3 position, RoomInformation containedRoom, float timeLimit, float priority, bool isinterruptible, int urgencyLevel, Func<bool> onDoneFunction)
    {
        this.urgencyLevel = urgencyLevel;
        this.taskName = name;
        this.taskScope = scope;
        this.target = position;
        this.timeLimit = timeLimit;
        this.priority = priority;
        this.isinterruptible = isinterruptible;
        this.onDoneFunction = onDoneFunction;
        this.containedRoom = containedRoom;
    }

    public WalkTask(string name, TaskScope scope, Vector3 position, RoomInformation containedRoom, float timeLim, float priority, bool isinterruptible, int urgencyLevel, Func<bool> onDoneFunction, eAnimationType type) : 
        this(name, scope, position, containedRoom, timeLim, priority, isinterruptible, urgencyLevel, onDoneFunction)
    {
        this.workAnimationType = type;
    }

    public WalkTask(string name, TaskScope scope, Vector3 position, RoomInformation containedRoom, float timeLim, float priority, bool isinterruptible, int urgencyLevel, Func<bool> onDoneFunction, eAnimationType type, List<ITask> followUpTasks) : 
        this(name, scope, position, containedRoom, timeLim, priority, isinterruptible, urgencyLevel, onDoneFunction, type)
    {
        this.followUpTasks = followUpTasks;
    }

    public Vector3 GetInteractionPosition => target;
    public Quaternion GetInteractionRotation => Quaternion.identity;

    public TaskScope GetTaskScope => taskScope;

    public eAnimationType GetWorkAnimationType => workAnimationType;

    public float GetPriority => priority;

    public float GetWorkTime => workTime;

    public bool IsWorkDone => true;
    public bool DoesWork => false;

    public List<ITask> FollowUpTasks { get { if (followUpTasks != null) { return followUpTasks; } else { return new List<ITask>(); } } }

    public bool IsInterruptible => isinterruptible;

    public string GetTaskInformation { get => "Walk Task: " + taskName + " " + GetInteractionPosition; set => taskName = value; }

    public bool IsTaskValid => isValid;

    public int GetTaskUrgencyLevel => urgencyLevel;


    public System.Func<bool> GetWorkDoneFunction => onDoneFunction;

    public Action onTaskInvalidate => OnTaskInvalidate;

    public RoomInformation GetInteractionRoom => containedRoom;

    public InteractableObject GetInteractableObject => null;

    public ITask GetRandomFollowUpTask()
    {
        if (followUpTasks != null && followUpTasks.Count > 0)
        {
            return followUpTasks[Random.Range(0, followUpTasks.Count)];
        }
        else
        {
            return null;
        }
    }

    public void SetTimeLimit(float time)
    {
        timeLimitTimer = time;
    }

    public void SetWorkTimer(float time)
    {
        workTimer = time;
    }

    public void UpdateTimeLimit(float deltaTime)
    {
        timeLimitTimer -= deltaTime;
    }

    public void UpdateWorkTimer(float deltaTime)
    {
        workTimer -= deltaTime;
    }

    public int CompareTo(ITask other)
    {
        if (other == null)
            return 1;

        else
            return priority.CompareTo(other.GetPriority);
    }
}
