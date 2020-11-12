using Pieter.GraphTraversal;
using Pieter.NavMesh;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class PersonTaskHandler
{
    private TaskListHolder globalTasks = null;
    private TaskListHolder privateTasks = null;

    public void DestroyTask()
    {
        activeTask = null;
    }

    private ITask activeTask = null;
    public ITask ActiveTask => activeTask;

    public string GetTaskName 
    { 
        get 
        { 
            if(activeTask != null)
            {
                return activeTask.GetTaskInformation;
            }
            else
            {
                return "Nothing";
            }
        } 
    }

    public void SetNewActiveTask(TraversalGenerator traversalGenerator, Vector3 personLocation, TraversalAStarNavigation traversalNav, ITask fallbackTask)
    {
        activeTask = GetNewTask(traversalGenerator, personLocation, traversalNav) ;
        if(activeTask == null)
        {
            activeTask = fallbackTask;
        }
    }

    public bool IsActiveTaskValid 
    { 
        get 
        {
            if (activeTask != null)
            {
                return activeTask.IsTaskValid;
            }
            else
            {
                return false;
            }
        } 
    }

    public bool IsActiveTaskInterruptible
    {
        get
        {
            if (activeTask != null)
            {
                return activeTask.IsInterruptible;
            }
            else
            {
                return true;
            }
        }
    }


    public PersonTaskHandler(TaskListHolder globalTasks, TaskListHolder privateTasks)
    {
        this.globalTasks = globalTasks;
        this.privateTasks = privateTasks;
    }

    public float DistanceToActiveTask(Vector3 pos)
    {
        if (activeTask != null && activeTask.IsTaskValid)
        {
            return Vector3.Distance(activeTask.GetInteractionPosition, pos);
        }
        else
        {
            return -1;
        }
    }

    public void ReturnActiveTask()
    {
        if(activeTask != null)
        {
            activeTask.GetTaskInformation = activeTask.GetTaskInformation + " Returned";
            AddTaskToScopedHolder(activeTask);
            activeTask = null;
        }
    }

    private void AddTaskToScopedHolder(ITask task)
    {
        switch (task.GetTaskScope)
        {
            case TaskScope.Global:
                globalTasks.AddTask(task);
                break;
            case TaskScope.Personal:
                privateTasks.AddTask(task);
                break;
        }
    }

    private void RemoveTaskInScopedHolder(ITask task)
    {
        switch (task.GetTaskScope)
        {
            case TaskScope.Global:
                globalTasks.Remove(task);
                break;
            case TaskScope.Personal:
                privateTasks.Remove(task);
                break;
        }
    }


    /// <summary>
    /// Get a new task from the list of tasks
    /// </summary>
    public ITask GetNewTask(TraversalGenerator traversalGenerator, Vector3 personLocation, TraversalAStarNavigation navigation)
    {
        List<ITask> allTasks = new List<ITask>();
        allTasks.AddRange(privateTasks.Tasks);
        allTasks.AddRange(globalTasks.Tasks);

        for (int i = allTasks.Count - 1; i >= 0; i--) 
        {
            if (!allTasks[i].IsTaskValid)
            {
                RemoveTaskInScopedHolder(allTasks[i]);
                allTasks.RemoveAt(i);
            }
        }

        if(allTasks.Count > 0)
        {
            List<NavMeshMovementLine> newPath = new List<NavMeshMovementLine>();
            for (int i = 0; i < allTasks.Count; i++)
            {
                newPath.Clear();
                //Quick check to see if the task is possible
                RoomInformation targetRoom = allTasks[i].GetInteractionRoom;
                if (targetRoom != null)
                {
                    Vertex targetVertex = targetRoom.TraversalGenerator.ClosestVertex(personLocation);
                    newPath = navigation.GetPathFromTo(traversalGenerator.ClosestVertex(targetVertex.Position), targetVertex);
                }
                if (newPath != null && newPath.Count > 0)
                {
                    ITask chosenTask = allTasks[i];
                    RemoveTaskInScopedHolder(allTasks[i]);
                    newPath.Clear();
                    return chosenTask;
                }
            }
        }
        return null;
    }

    /// <summary>
    /// Get a new task from the list of follow up tasks from the current task
    /// </summary>
    public void SetNewFollowUpTask()
    {
        if (activeTask.FollowUpTasks.Count > 0)
        {
            float highestPriority = activeTask.FollowUpTasks[0].GetPriority;
            ITask selectedTask = activeTask.FollowUpTasks[0];
            foreach (ITask newTask in activeTask.FollowUpTasks)
            {
                if (highestPriority < newTask.GetPriority)
                {
                    highestPriority = newTask.GetPriority;
                    selectedTask = newTask;
                }
            }

            activeTask = selectedTask;
        }
        else
        {
            activeTask = null;
        }
    }

    public void AddNewTask(ITask task)
    {
        privateTasks.AddTask(task);
    }
}
