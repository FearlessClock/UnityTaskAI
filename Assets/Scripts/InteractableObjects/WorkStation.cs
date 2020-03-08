using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorkStation : InteractableObject
{
    [SerializeField] protected TaskListHolder taskList = null;
    [SerializeField] private float timeTillTaskGenerate = 3;
    private float taskGenerateTimer = 0;
    [SerializeField] private float workstationPriority = 10;
    [SerializeField] private float workstationWorkTime = 10;
    private void Awake()
    {
        taskGenerateTimer = timeTillTaskGenerate;
        taskList.AddTask(GenerateTask());
    }

    private void Update()
    {
        taskGenerateTimer -= Time.deltaTime;
        if(taskGenerateTimer < 0)
        {
            taskGenerateTimer = timeTillTaskGenerate;
            taskList.AddTask(GenerateTask());
        }
    }
    public override TaskBase GenerateTask()
    {
        return new TaskBase(interactionPoint, 10, 1, 3, eAnimationType.Work, this);
    }
}
