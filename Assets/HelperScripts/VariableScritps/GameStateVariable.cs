using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public enum GameState { MainMenu, Moving, Planning, Pause, GameOver, Victory }
[CreateAssetMenu(fileName = "GameStateVariable", menuName = "UnityHelperScripts/GameStateVariable", order = 0)]
public class GameStateVariable : ScriptableObject
{
    public GameState value;
    public UnityEngine.Events.UnityEvent OnValueChanged;
    public static implicit operator GameState(GameStateVariable reference)
    {
        return reference.value;
    }

    public void SetValue(GameState v)
    {
        this.value = v;
        OnValueChanged?.Invoke();
    }

    public override string ToString()
    {
        return value.ToString();
    }
}
