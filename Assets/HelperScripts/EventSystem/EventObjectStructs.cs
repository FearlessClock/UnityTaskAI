using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct TwoObject
{
    public object attackee;
    public object attacked;
}

[System.Serializable]
public struct IsMasterStruct
{
    public bool isMaster;
    public bool state;
}
