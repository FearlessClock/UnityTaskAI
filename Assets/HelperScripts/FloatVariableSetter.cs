using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloatVariableSetter : MonoBehaviour
{
    [SerializeField] private FloatVariable variable = null;
    [SerializeField] private float value = 0;
    private void Awake()
    {
        ResetVariable();
    }

    public void ResetVariable()
    {
        variable.SetValue(value);
    }
}
