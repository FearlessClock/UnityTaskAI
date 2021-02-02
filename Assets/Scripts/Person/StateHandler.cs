using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateHandler 
{
    // The persons current state 
    private Stack<ePersonState> stateStack = new Stack<ePersonState>();
    private ePersonState CurrentState { get { if (stateStack.Count > 0) return stateStack.Peek(); else return ePersonState.Idle; } }

    public string GetCurrentStateName => CurrentState.ToString();

    private ePersonState startingState = ePersonState.Idle;
    public ePersonState SetStartingState
    {
        get { return startingState; }
        set { startingState = value; }
    }

    private Dictionary<ePersonState, State> stateDict = new Dictionary<ePersonState, State>();

    public StateHandler(State[] states)
    {
        for (int i = 0; states != null && i < states.Length; i++)
        {
            stateDict.Add(states[i].stateEnum, states[i]);
        }
    }

    public void AddState(ePersonState stateEnum, StateTransition entry, StateTransition exit, StateUpdate state)
    {
        State newState = new State { stateEnum = stateEnum, entryFunction = entry, exitFunction = exit, stateFunction = state };
        stateDict.Add(stateEnum, newState);
    }

    public void TransitionToState(ePersonState nextState)
    {
        if(stateStack.Count > 0)
        {
            stateDict[CurrentState].exitFunction();
        }
        stateStack.Push(nextState);
        stateDict[CurrentState].entryFunction();
    }

    public void FinishState()
    {
        stateDict[CurrentState].exitFunction();
        stateStack.Pop();
        stateDict[CurrentState].entryFunction();
    }

    public void Update()
    {
        if (stateDict[CurrentState].stateFunction())
        {
            FinishState();
        }
    }

    public void ResetStates()
    {
        stateStack = new Stack<ePersonState>();
        TransitionToState(startingState);
    }
}

public delegate void StateTransition();
public delegate bool StateUpdate();
public class State
{
    public ePersonState stateEnum;
    public StateTransition entryFunction;
    public StateTransition exitFunction;
    public StateUpdate stateFunction;
}
