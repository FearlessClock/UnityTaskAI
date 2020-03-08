using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

[CreateAssetMenu(fileName = "QuaternionVariable", menuName = "UnityHelperScripts/QuaternionVariable", order = 0)]
class QuaternionVariable : ScriptableObject
{
    public Quaternion value;

    public void SetValue(Quaternion v)
    {
        this.value = v;
    }

    public override string ToString()
    {
        return value.ToString();
    }
}