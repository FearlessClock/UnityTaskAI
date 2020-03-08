using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Handles the task list and dispaches the tasks
/// </summary>
public class TaskDispatcher : MonoBehaviour
{
    [SerializeField] private TaskListHolder taskList = null;

    private void Awake()
    {
        taskList.Clear();
    }
    private void Update()
    {
        foreach (TaskBase task in taskList.GetAllTasks)
        {
            task.UpdateTimeLimit(Time.deltaTime);
        }
    }


}
