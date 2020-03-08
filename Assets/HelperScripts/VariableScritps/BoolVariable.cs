using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

[CreateAssetMenu(fileName = "BoolVariable", menuName = "UnityHelperScripts/BoolVariable", order = 0)]
public class BoolVariable : ScriptableObject
{
    public bool value = false;
    public BoolEvent OnValueChanged;
    public static implicit operator bool(BoolVariable reference)
    {
        return reference.value;
    }

    public void SetValue(bool v)
    {
        this.value = v;
        OnValueChanged?.Invoke(v);
    }

    public void Inverse()
    {
        this.value = !this.value;
    }

    public override string ToString()
    {
        return value.ToString();
    }
}