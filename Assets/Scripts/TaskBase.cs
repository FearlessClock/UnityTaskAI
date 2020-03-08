using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class holding the information for the task system
/// </summary>
[System.Serializable]
public class TaskBase
{
    public Transform interactionPoint;
    public eAnimationType workAnimationType;
    public float workTime;
    public float workTimer = 0;
    public float timeLimit;
    private float timeLimitTimer = 0;
    public float priority;
    public InteractableObject interactableObject;
    public List<TaskBase> followUpTasks;

    public TaskBase(Transform transform, float timeLim, float prio, float workTime)
    {
        this.interactionPoint = transform;
        this.timeLimit = timeLim;
        this.timeLimitTimer = timeLimit;
        this.priority = prio;
        this.workTime = workTime;
    }

    public TaskBase(Transform transform, float timeLim, float prio, float workTime, eAnimationType type) : 
            this(transform, timeLim, prio, workTime)
    {
        this.interactionPoint = transform;
        this.timeLimit = timeLim;
        this.priority = prio;
        this.workTime = workTime;
        this.workAnimationType = type;
    }

    public TaskBase(Transform transform, float timeLim, float prio, float workTime, eAnimationType type, InteractableObject interObject) : 
            this(transform, timeLim, prio, workTime, type)
    {
        this.interactableObject = interObject;
    }

    public TaskBase(Transform transform, float timeLim, float prio, float workTime, eAnimationType type, InteractableObject interObject, List<TaskBase> followUpTasks) : 
            this(transform, timeLim, prio, workTime, type, interObject)
    {
        this.followUpTasks = followUpTasks;
    }

    public List<TaskBase> FollowUpTasks { get { if (followUpTasks != null) { return followUpTasks; } else { return new List<TaskBase>(); } } }

    public TaskBase GetRandomFollowUpTask()
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

    public bool IsWorkDone()
    {
        return workTimer < 0;
    }

    public void UpdateTimeLimit(float deltaTime)
    {
        timeLimitTimer -= Time.deltaTime;
    }
}
