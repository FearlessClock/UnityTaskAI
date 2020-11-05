using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class WorldSpaceTextDebug : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI textMesh = null;

    private Dictionary<string, string> writtenInfo = new Dictionary<string, string>();
    public void Write(string id, string info)
    {
        if (writtenInfo.ContainsKey(id))
        {
            writtenInfo[id] = info;
        }
        else
        {
            writtenInfo.Add(id, info);
        }
        WriteToText();
    }

    private void WriteToText()
    {
        string text = "";
        foreach (string item in writtenInfo.Values)
        {
            text += item + "\n";
        }

        textMesh.SetText(text);
    }
}
