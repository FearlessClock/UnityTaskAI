using UnityEngine;

[CreateAssetMenu(fileName = "AxisVariable", menuName = "UnityHelperScripts/AxisVariable", order = 0)]
public class AxisVariable : ScriptableObject {
    public string axisName;

    public float GetRawAxis(){
        return Input.GetAxisRaw(axisName);
    }
}