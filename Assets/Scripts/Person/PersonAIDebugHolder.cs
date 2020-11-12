using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum eDebugImportance { Entry, Exit, State, Error, Unimportant, Important}

[CreateAssetMenu(menuName = "TaskAI/PersonAIDebugHolder")]
public class PersonAIDebugHolder : ScriptableObject
{
    public int maxLines = 3000;
    public List<string> debugText = new List<string>();
    public List<Color> debugColor = new List<Color>();
    
    public void Log(string text, eDebugImportance importance)
    {
        text = "(" + Time.realtimeSinceStartup + ") " + text;
        if(debugText.Count >= maxLines)
        {
            debugText.RemoveAt(0);
            debugColor.RemoveAt(0);
        }
        debugText.Add(text);
        debugColor.Add(GetDebugColor(importance));
    }

    private Color GetDebugColor(eDebugImportance enumForColor)
    {
        switch (enumForColor)
        {
            case eDebugImportance.Entry:
                return Color.cyan;
            case eDebugImportance.Exit:
                return Color.magenta;
            case eDebugImportance.State:
                return Color.yellow;
            case eDebugImportance.Error:
                return Color.red;
            case eDebugImportance.Unimportant:
                return Color.gray;
            case eDebugImportance.Important:
                return Color.green;
            default:
                return Color.gray;
        }
    }

    internal void Log(object p, eDebugImportance unimportant)
    {
        throw new NotImplementedException();
    }
}
