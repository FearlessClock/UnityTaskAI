
using System;
using UnityEngine;

public class Human : PersonBase
{

    private void OnDestroy()
    {
        taskHandler.DestroyTask();    
    }

    private void Awake()
    {
        StartUp();
    }

    private void OnEnable()
    {
        ScientistCounterController.instance.AddScientist();
    }

    protected override void OnDeath()
    {
        ScientistCounterController.instance.KillScientist();
    }

    private void Update()
    {
        if (!CheckIfAlive())
        {
            return;
        }
        CheckIfOnFire();
        if (taskHandler.IsActiveTaskValid)
        {
            WorkOnTask();
        }
        else
        {
            if (!taskHandler.SetNewActiveTask(movementHandler.GetCurrentRoom.TraversalGenerator, this.transform.position, traversalAStar))
            {
                WanderTask task = new WanderTask("Wander " + movementHandler.GetCurrentRoom.name, TaskScope.Personal, movementHandler.GetCurrentRoom.GetRandomSpotInsideRoom, movementHandler.GetCurrentRoom, null, 2, 5, true, 1, 3);
                debugHolder.Log("No active and available tasks found, setting wander as active", eDebugImportance.Unimportant);
                taskHandler.SetActiveTask(task);
            }

        }

        UpdateDebug();
    }

    public void AddNewTask(ITask basicTask)
    {
        taskHandler.AddNewTask(basicTask);
    }

    private void OnDrawGizmos()
    {
        if(taskHandler != null && taskHandler.IsActiveTaskValid)
        {
            Gizmos.color = Color.red;
            
            Gizmos.DrawWireSphere(taskHandler.ActiveTask.GetInteractionPosition, 0.2f);
        }

        base.PersonBaseOnDrawGizmos();
    }
}