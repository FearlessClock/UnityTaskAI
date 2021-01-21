using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public enum TaskScope { Global, Personal}
/// <summary>
/// Class holding the information for the task system
/// </summary>
[System.Serializable]
public class BasicTask : ITask
{
    private TaskScope taskScope = TaskScope.Global;
    private Transform interactionPoint = null;
    private RoomInformation containedRoom = null;

    private eAnimationType workAnimationType = eAnimationType.Idle;
    private float workTime = 0;
    private float workTimer = 0;
    private float timeLimit = 0;
    private float timeLimitTimer = 0;
    private float priority = 0;
    private InteractableObject interactableObject = null;
    private List<ITask> followUpTasks = null;
    private bool isinterruptible = true;
    private bool isValid = true;
    private int UrgencyLevel = 1;
    private Func<bool> onDoneFunction = null;
    private Action OnTaskInvalidated = null;
    private string taskName = "";
    public Vector3 GetInteractionPosition { get { if (interactionPoint != null) return interactionPoint.position; else { isValid = false; onTaskInvalidate?.Invoke(); return Vector3.zero; } } }
    public Quaternion GetInteractionRotation { get { if (interactionPoint != null) return interactionPoint.rotation; else { isValid = false; onTaskInvalidate?.Invoke(); return Quaternion.identity; } } }

    public TaskScope GetTaskScope => taskScope;

    public eAnimationType GetWorkAnimationType => workAnimationType;

    public float GetPriority => priority;

    public bool IsWorkDone => workTimer < 0;
    public float GetWorkTime => workTime;

    public void UpdateTimeLimit(float deltaTime) => timeLimitTimer -= Time.deltaTime;
    public void SetTimeLimit(float time) => this.timeLimit = time;

    public void SetWorkTimer(float time) => this.workTimer = time;

    public void UpdateWorkTimer(float deltaTime) => workTimer -= deltaTime;

    public BasicTask(string name, TaskScope scope, Transform transform, RoomInformation containedRoom, float timeLim, float prio, float workTime, bool isinterruptible, int urgency, Func<bool> onDoneFunction)
    {
        this.UrgencyLevel = urgency;
        this.taskName = name;
        this.taskScope = scope;
        this.interactionPoint = transform;
        this.timeLimit = timeLim;
        this.timeLimitTimer = timeLimit;
        this.priority = prio;
        this.workTime = workTime;
        this.isinterruptible = isinterruptible;
        this.onDoneFunction = onDoneFunction;
        this.containedRoom = containedRoom;
    }

    public BasicTask(string name, TaskScope scope, Transform transform, RoomInformation containedRoom, float timeLim, float prio, float workTime, bool isinterruptible, int urgency, Func<bool> OnDoneFunction, eAnimationType type) :
            this(name, scope, transform, containedRoom, timeLim, prio, workTime, isinterruptible, urgency, OnDoneFunction)
    {
        this.workAnimationType = type;
    }

    public BasicTask(string name, TaskScope scope, Transform transform, RoomInformation containedRoom, float timeLim, float prio, float workTime, bool isinterruptible, int urgency, Func<bool> OnDoneFunction, eAnimationType type, InteractableObject interObject) : 
            this(name, scope, transform, containedRoom, timeLim, prio, workTime, isinterruptible, urgency, OnDoneFunction, type)
    {
        this.interactableObject = interObject;
    }

    public BasicTask(string name, TaskScope scope, Transform transform, RoomInformation containedRoom, float timeLim, float prio, float workTime, bool isinterruptible, int urgency, Func<bool> OnDoneFunction, eAnimationType type, InteractableObject interObject, List<ITask> followUpTasks) : 
            this(name, scope, transform, containedRoom, timeLim, prio, workTime, isinterruptible, urgency, OnDoneFunction, type, interObject)
    {
        this.followUpTasks = followUpTasks;
    }

    public List<ITask> FollowUpTasks { get { if (followUpTasks != null) { return followUpTasks; } else { return new List<ITask>(); } } }

    public bool IsInterruptible => isinterruptible;

    public string GetTaskInformation { get => "Task Base: " + taskName + " " + GetInteractionPosition; set => taskName = value; }

    public bool IsTaskValid => isValid && (interactableObject != null && interactableObject.IsWorking);
    public bool DoesWork => true;

    public int GetTaskUrgencyLevel => UrgencyLevel;

    public System.Func<bool> GetWorkDoneFunction => onDoneFunction;

    public Action onTaskInvalidate => OnTaskInvalidated;

    public RoomInformation GetInteractionRoom => containedRoom;

    public InteractableObject GetInteractableObject => interactableObject;

    public ITask GetRandomFollowUpTask()
    {
        if(followUpTasks != null && followUpTasks.Count > 0)
        {
            return followUpTasks[Random.Range(0, followUpTasks.Count)];
        }
        else
        {
            return null;
        }
    }

    public int CompareTo(ITask other)
    {
        if (other == null)
            return 1;

        else
            return priority.CompareTo(other.GetPriority);
    }
}
