
using System;
using UnityEngine;

public class Human : PersonBase
{
    [SerializeField] Bed bed = null;
    private float tiredLevel = 10;
    private float tiredBecoming = 0.1f;
    private TaskBase sleepTask = null;

    private void OnDestroy()
    {
        currentTask = null;    
    }

    private void Awake()
    {
        StartUp();
    }

    private void Update()
    {
        if (!CheckIfAlive())
        {
            return;
        }
        CheckIfOnFire();
        if (currentTask != null)
        {
            WorkOnTask();
        }
        else
        {
            currentTask = GetNewTask();
        }

        UpdateDebug();
        if (bed != null)
        {
            tiredLevel -= tiredBecoming;
            if (tiredLevel < 5)
            {
                if (sleepTask == null)
                {
                    sleepTask = bed.GenerateTask();
                    AddNewTask(sleepTask);
                }
            }
        }
    }

    private void OnDrawGizmos()
    {
        if(currentTask != null)
        {
            Gizmos.color = Color.red;
            
            Gizmos.DrawWireSphere(currentTask.GetInteractionPosition, 0.2f);
        }

        base.PersonBaseOnDrawGizmos();
    }
}