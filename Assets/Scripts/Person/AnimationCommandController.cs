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

    [SerializeField] private string commandsDebug = "";

    private void Update()
    {
        commandsDebug = commandList.Count + " ";
        foreach (eAnimationType ani in commandList)
        {
            commandsDebug += ani.ToString();
        }
    }

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
                UpdateAnimator();
            }
            else
            {
                UpdateCommand();
            }
        }
    }

    private void UpdateAnimator()
    {
        switch (currentCommand)
        {
            case eAnimationType.Idle:
                break;
            case eAnimationType.Work:
                animator.SetTrigger(currentCommand.ToString());
                break;
            case eAnimationType.Sleep:
                animator.SetTrigger(currentCommand.ToString());
                break;
            case eAnimationType.Panic:
                animator.SetTrigger(currentCommand.ToString());
                break;
            case eAnimationType.Walk:

                break;
            case eAnimationType.Death:
                animator.SetTrigger(currentCommand.ToString());
                break;
            default:
                break;
        }
    }

    public void SetCurrentState(eAnimationType selectedState)
    {
        currentState = selectedState;
        UpdateCommand();
    }
}
