using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Set a bool variable to a defined amount on awake
/// </summary>
public class BoolVariableSetter : MonoBehaviour
{
    [SerializeField] private BoolVariable variable = null;
    [SerializeField] private bool setValue = false;
    private void Awake()
    {
        variable.SetValue(setValue);
    }
}
