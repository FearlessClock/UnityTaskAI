using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class StringVariableEvent : UnityEvent<StringVariable> { }
[CreateAssetMenu(fileName = "StringVariable", menuName = "UnityHelperScripts/StringVariable", order = 0)]
public class StringVariable : ScriptableObject {
    public string value;

    public StringVariableEvent OnValueChanged;
    public static implicit operator string(StringVariable reference)
    {
            return reference.value;
    }

    public void SetValue(string v)
    {
        this.value = v;
        OnValueChanged?.Invoke(this);
    }

    public override string ToString()
    {
        return value.ToString();
    }
}