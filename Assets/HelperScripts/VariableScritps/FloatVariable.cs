using System;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "FloatVariable", menuName = "UnityHelperScripts/FloatVariable", order = 0)]
public class FloatVariable : ScriptableObject 
{
    public float value;
    public UnityEvent OnValueChanged;
    public static implicit operator float(FloatVariable reference)
    {
            return reference.value;
    }

    public void SetValue(float v)
    {
        this.value = v;
        OnValueChanged?.Invoke();
    }

    public void Add(float v)
    {
        this.value += v;
        OnValueChanged?.Invoke();
    }

    public override string ToString(){
        return value.ToString();
    }
}