using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public enum eSMState {Moving, Diving}
public class eStateEvent:UnityEvent<eSMState>{}
[CreateAssetMenu(fileName = "StateMachineVariable", menuName = "UnityHelperScripts/StateMachineVariable", order = 0)]
public class StateMachineVariable : ScriptableObject {
    [SerializeField] private eSMState currentState;
    public eStateEvent OnStateChange;
    public eStateEvent OnStateExit;

    public eSMState GetCurrentState(){
        return currentState;
    }

    public void SetStateTo(eSMState newState){
        OnStateExit?.Invoke(currentState);
        currentState = newState;
        OnStateChange?.Invoke(currentState);
    }

    
    public static implicit operator eSMState(StateMachineVariable reference)
    {
            return reference.currentState;
    }
}