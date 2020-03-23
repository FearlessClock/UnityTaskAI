using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Holds a list of all the tasks that need to be performed
/// </summary>
[System.Serializable]
[CreateAssetMenu(menuName = "TaskAI/Task List Holder")]
public class TaskListHolder : ScriptableObject
{
    [SerializeField] private List<TaskBase> tasks = new List<TaskBase>();

    public IEnumerable<TaskBase> GetAllTasks => tasks.ToArray();

    public void AddTask(TaskBase newTask)
    {
        tasks.Add(newTask);
        tasks.Sort((t1, t2) => t1.priority.CompareTo(t2.priority));
    }

    public void Clear()
    {
        tasks.Clear();
    }

    public TaskBase GetHighestPriorityTask()
    {
        if(tasks.Count > 0)
        {
            TaskBase task = tasks[0];
            tasks.RemoveAt(0);
            return task;
        }
        return null;
    }

    public TaskBase PeekHighestPriorityTask()
    {
        if(tasks.Count > 0)
        {
            return tasks[0];
        }
        else
        {
            return null;
        }
    }
}
