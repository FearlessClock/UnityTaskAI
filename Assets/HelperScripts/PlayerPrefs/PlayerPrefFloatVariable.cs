using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New PlayerPrefFloatVariable", menuName = "UnityHelperScripts/PlayerPrefs/PlayerPrefFloatVariable", order = 0)]
public class PlayerPrefFloatVariable : ScriptableObject
{
    public string ID;
    public float value;

    private void OnEnable()
    {
        Load();
    }

    public void Save()
    {
        PlayerPrefs.SetFloat(ID, value);
    }

    public void Load()
    {
        value = PlayerPrefs.GetFloat(ID);
    }

    public void SetValue(float value)
    {
        this.value = value;
        Save();
    }

    [System.Obsolete("Use LatestValue instead, moving to properties and so this function was deprecated")]
    public float GetLatestValue()
    {
        Load();
        return value;
    }

    public float LatestValue
    {
        get { Load(); return value; }
    }
}
