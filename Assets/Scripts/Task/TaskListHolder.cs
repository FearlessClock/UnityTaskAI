using System;
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
    private List<ITask> tasks = new List<ITask>();

    public IEnumerable<ITask> GetAllTasks => tasks.ToArray();
    public ITask[] Tasks => tasks.ToArray();
    public int Length => tasks.Count;

    public void AddTask(ITask newTask)
    {
        tasks.Add(newTask);
        tasks.Sort((t1, t2) => t1.GetPriority.CompareTo(t2.GetPriority));
    }

    public void Clear()
    {
        tasks.Clear();
    }

    public ITask GetHighestPriorityTask()
    {
        if(tasks.Count > 0)
        {
            ITask task = tasks[0];
            tasks.RemoveAt(0);
            return task;
        }
        return null;
    }

    public ITask PeekHighestPriorityTask()
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

    public void Remove(ITask task)
    {
        tasks.Remove(task);
    }
}
