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
    [SerializeField] private bool generateOnStartup = false;
    private void Start()
    {
        taskGenerateTimer = timeTillTaskGenerate;
        if (generateOnStartup)
        {
            taskList.AddTask(GenerateTask());
        }
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
    public override BasicTask GenerateTask()
    {
        return new BasicTask("WorkStation-"+this.name, TaskScope.Global, interactionPoint, roomGraphHolder.FindRoomAtLocation(interactionPoint.position), 10, 1, 3, true, 1, null, eAnimationType.Work, this) ;
    }
}
