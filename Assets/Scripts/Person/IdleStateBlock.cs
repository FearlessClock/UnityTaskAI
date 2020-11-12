using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleStateBlock : StateBlock
{
    private PersonAIDebugHolder debugHolder = null;
    private AnimationCommandController animatorController = null;
    private PersonTaskHandler taskHandler = null;
    private StateHandler stateHandler = null;
    private float minDistanceToTaskLocation = 0.3f;
    private Transform playerLocation = null;

    public IdleStateBlock(PersonAIDebugHolder debug, AnimationCommandController animator, PersonTaskHandler taskHandler,
                            StateHandler stateHandler, float minDistanceToTaskLocation, Transform playerLocation)
    {
        debugHolder = debug;
        animatorController = animator;
        this.taskHandler = taskHandler;
        this.stateHandler = stateHandler;
        this.minDistanceToTaskLocation = minDistanceToTaskLocation;
        this.playerLocation = playerLocation;
    }
    public void Entry()
    {
        debugHolder.Log("Idle Entry Called", eDebugImportance.Entry);
        animatorController.ChangeState(eAnimationType.Idle);
    }

    public void Exit()
    {
        debugHolder.Log("Idle Exit Called", eDebugImportance.Exit);
    }

    public bool Update()
    {
        if (!taskHandler.IsActiveTaskValid)
        {
            return false;
        }
        float distanceToTaskPosition = taskHandler.DistanceToActiveTask(playerLocation.position);

        if (distanceToTaskPosition >= 0 && distanceToTaskPosition >= minDistanceToTaskLocation)
        {
            stateHandler.TransitionToState(ePersonState.Move);
        }
        else if (distanceToTaskPosition >= 0 && distanceToTaskPosition < minDistanceToTaskLocation)
        {
            stateHandler.TransitionToState(ePersonState.Work);
        }
        return false; // Always returns false because it is always the first state so it cannot FinishState()
    }
}
