using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationCommandController : MonoBehaviour
{
    [SerializeField] private Animator animator = null;
    private Queue<eAnimationType> commandList = new Queue<eAnimationType>();
    private eAnimationType currentCommand = eAnimationType.Idle;

    private eAnimationType currentState = eAnimationType.Idle;

    public void ChangeState(eAnimationType type)
    {
        commandList.Enqueue(type);
        if(currentCommand == currentState)
        {
            UpdateCommand();
        }
    }

    public void SetFloat(string name, int value)
    {
        animator.SetFloat(name, value);
    }

    private void UpdateCommand()
    {
        if (commandList.Count > 0)
        {
            currentCommand = commandList.Dequeue();
            if(currentCommand != currentState)
            {
                animator.SetTrigger(currentCommand.ToString());
            }
        }
    }

    public void SetCurrentState(eAnimationType selectedState)
    {
        currentState = selectedState;
        UpdateCommand();
    }
}
