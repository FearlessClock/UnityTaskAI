using System;
using UnityEngine;

[CreateAssetMenu(fileName = "Vector3Variable", menuName = "UnityHelperScripts/Vector3Variable", order = 0)]
public class Vector3Variable : ScriptableObject {
    public Vector3 value;
    public Vector3Event OnValueChanged;

    public void SetValue(Vector3 position)
    {
        value = position;
        OnValueChanged?.Invoke(value);
    }
}