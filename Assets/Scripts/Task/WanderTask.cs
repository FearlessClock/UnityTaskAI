﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class WanderTask : ITask
{
    private TaskScope taskScope = TaskScope.Global;
    private Vector3 target;

    private eAnimationType workAnimationType = eAnimationType.Idle;
    private Func<bool> onWorkDonefunction = null;
    private Func<bool> isTaskStillValidFunction = null;
    private float workTime = 0;
    private float workTimer = 0;
    private float timeLimit = 0;
    private float timeLimitTimer = 0;
    private float priority = 0;
    private List<TaskBase> followUpTasks = new List<TaskBase>();
    private bool isinterruptible = true;
    private bool isValid = true;
    private int urgencyLevel = 1;
    public WanderTask(string name, TaskScope scope, Vector3 position, Func<bool> isTaskStillValidFunction, float timeLimit, float priority, bool isinterruptible, int urgencyLevel, float waitAroundTime)
    {
        this.workTime = waitAroundTime;
        this.urgencyLevel = urgencyLevel;
        this.taskName = name;
        this.taskScope = scope;
        this.target = position;
        this.timeLimit = timeLimit;
        this.priority = priority;
        this.isinterruptible = isinterruptible;
        this.workTimer = this.workTime;
        this.isTaskStillValidFunction = isTaskStillValidFunction;
    }

    public WanderTask(string name, TaskScope scope, Vector3 position, Func<bool> isTaskStillValidFunction, float timeLim, float priority, bool isinterruptible, int urgencyLevel, float waitAroundTime, eAnimationType type) :
        this(name, scope, position, isTaskStillValidFunction, timeLim, priority, isinterruptible, urgencyLevel, waitAroundTime)
    {
        this.workAnimationType = type;
    }

    public WanderTask(string name, TaskScope scope, Vector3 position, Func<bool> isTaskStillValidFunction, float timeLim, float priority, bool isinterruptible, int urgencyLevel, float waitAroundTime, eAnimationType type, List<TaskBase> followUpTasks) :
        this(name, scope, position, isTaskStillValidFunction, timeLim, priority, isinterruptible, urgencyLevel, waitAroundTime, type)
    {
        this.followUpTasks = followUpTasks;
    }
    public WanderTask(string name, TaskScope scope, Vector3 position, Func<bool> isTaskStillValidFunction, float timeLim, float priority, bool isinterruptible, int urgencyLevel, float waitAroundTime, eAnimationType type, Func<bool> onWorkDoneFunction) :
        this(name, scope, position, isTaskStillValidFunction, timeLim, priority, isinterruptible, urgencyLevel, waitAroundTime)
    {
        this.workAnimationType = type;
        this.onWorkDonefunction = onWorkDoneFunction;
    }

    public Vector3 GetInteractionPosition => target;
    public Quaternion GetInteractionRotation => Quaternion.identity;

    public TaskScope GetTaskScope => taskScope;

    public eAnimationType GetWorkAnimationType => workAnimationType;

    public float GetPriority => priority;

    public float GetWorkTime => workTime;

    public bool IsWorkDone => workTimer < 0;
    public bool DoesWork => true;

    public List<TaskBase> FollowUpTasks { get { if (followUpTasks != null) { return followUpTasks; } else { return new List<TaskBase>(); } } }

    public bool Isinterruptible => isinterruptible;

    public string GetTaskInformation { get => "Wander Task: " + taskName + " " + GetInteractionPosition; set => taskName = value; }

    public bool IsTaskValid
    {
        get
        {
            if(isTaskStillValidFunction != null)
            {
                return isTaskStillValidFunction() && isValid;
            }
            else
            {
                return isValid;
            }
        }
    }

    public int GetTaskUrgencyLevel => urgencyLevel;

    public System.Func<bool> GetWorkDoneFunction => onWorkDonefunction;

    public string taskName = "";

    public TaskBase GetRandomFollowUpTask()
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
}
