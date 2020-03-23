
using UnityEngine;

public class Human : PersonBase
{
    [SerializeField] Bed bed = null;
    private float tiredLevel = 10;
    private float tiredBecoming = 0.1f;
    private TaskBase sleepTask = null;
    private void Awake()
    {
        StartUp();
        if(currentTask == null)
        {
            currentTask = GetNewTask();
        }
    }

    private void Update()
    {
        if(currentTask != null)
        {
            WorkOnTask();
        }
        else
        {
            currentTask = GetNewTask();
        }

        if(bed != null)
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
            
            Gizmos.DrawWireSphere(currentTask.interactionPoint.position, 0.2f);
        }
    }
}