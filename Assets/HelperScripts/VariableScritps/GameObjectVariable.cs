using System;
using UnityEngine;

[CreateAssetMenu(fileName = "GameObjectVariable", menuName = "UnityHelperScripts/GameObjectVariable", order = 0)]
public class GameObjectVariable : ScriptableObject
{
    public GameObject value;
    public GameObjectEvent OnValueChanged;
    public static implicit operator GameObject(GameObjectVariable reference)
    {
        return reference.value;
    }

    public void SetValue(GameObject v)
    {
        this.value = v;
        OnValueChanged?.Invoke(v);
    }

    public override string ToString()
    {
        return value.name.ToString();
    }
}