using DG.Tweening.Plugins.Core.PathCore;
using Pieter.GraphTraversal;
using Pieter.NavMesh;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveStateBlock : StateBlock
{
    private PersonAIDebugHolder debugHolder = null;
    private AnimationCommandController animatorController = null;
    private PersonTaskHandler taskHandler = null;
    private TraversalAStarNavigation traversalNav = null;
    private MovementHandler movementHandler = null;

    public MoveStateBlock(PersonAIDebugHolder debug, AnimationCommandController animator, PersonTaskHandler taskHandler,
                            TraversalAStarNavigation traversalNav, MovementHandler movementHandler)
    {
        this.debugHolder = debug;
        this.animatorController = animator;
        this.taskHandler = taskHandler;
        this.traversalNav = traversalNav;
        this.movementHandler = movementHandler;
    }

    public void Entry()
    {
        debugHolder.Log("GOTO Entry Called for Task " + taskHandler.ActiveTask.GetTaskInformation, eDebugImportance.Entry);
        RoomInformation startingRoom = movementHandler.GetCurrentRoom;
        RoomInformation endingRoom = taskHandler.ActiveTask.GetInteractionRoom;

        if (movementHandler.SetPathFromPlayerTo(taskHandler.ActiveTask.GetInteractionPosition, startingRoom, endingRoom, traversalNav))
        {
            debugHolder.Log("Found path of length " + movementHandler.Path.Count, eDebugImportance.Unimportant);
            animatorController.ChangeState(eAnimationType.Walk);
            animatorController.SetFloat("SpeedMultiplier", taskHandler.ActiveTask.GetTaskUrgencyLevel);
            movementHandler.SetPathIndexToZero();
            movementHandler.SetPathToTargetToFirst();
            movementHandler.SetActiveTask(taskHandler.ActiveTask);
            debugHolder.Log("Path length " + movementHandler.Path.Count, eDebugImportance.Unimportant);

        }
        else
        {
            debugHolder.Log("Failed to set path " + (movementHandler.Path != null? movementHandler.Path.Count.ToString() : ""), eDebugImportance.Error);
        }
    }

    public void Exit()
    {
        debugHolder.Log("Clearing movement stack", eDebugImportance.Exit);
        movementHandler.ClearPath();
        debugHolder.Log("GOTO Exit Called", eDebugImportance.Exit);
    }

    public bool Update()
    {
        if (!taskHandler.IsActiveTaskValid)
        {
            return false;
        }
        if(movementHandler.CheckPathIsEmpty())
        {
            debugHolder.Log("Path was empty", eDebugImportance.Error);
            debugHolder.Log("Returning active task " + taskHandler.ActiveTask.GetTaskInformation, eDebugImportance.Error);
            taskHandler.ReturnActiveTask();
            return true;
        }
        // No path was found to the target task
        if (!movementHandler.IsValid)
        {
            taskHandler.ReturnActiveTask();
            return true;
        }
        else
        {
            int res = movementHandler.MoveToTarget(taskHandler.ActiveTask.GetTaskUrgencyLevel);
            switch (res)
            {
                case 1:
                    return true;
                case 0:
                    return false;
                case -1:
                    taskHandler.ReturnActiveTask();
                    return true;
            }
            return true;
        }
    }
}
