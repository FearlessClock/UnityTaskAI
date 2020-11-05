using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ITask
{
    Func<bool> GetWorkDoneFunction { get; }
    int GetTaskUrgencyLevel { get; }
    bool DoesWork { get; }
    bool IsTaskValid { get; }
    string GetTaskInformation { get; set; }
    bool Isinterruptible { get; }
    Vector3 GetInteractionPosition { get; }
    Quaternion GetInteractionRotation { get; }
    TaskScope GetTaskScope { get; }
    eAnimationType GetWorkAnimationType { get; }
    float GetPriority { get; }
    void SetWorkTimer(float time);
    float GetWorkTime { get; }
    bool IsWorkDone { get; }
    List<TaskBase> FollowUpTasks { get; }

    TaskBase GetRandomFollowUpTask();


    void UpdateTimeLimit(float deltaTime);
    void SetTimeLimit(float time);
    void UpdateWorkTimer(float deltaTime);

}
