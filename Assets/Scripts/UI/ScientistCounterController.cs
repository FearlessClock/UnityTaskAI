using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ScientistCounterController : MonoBehaviour
{
    public static ScientistCounterController instance = null;

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this);
        }

        UpdateText();
    }

    private int total = 0;
    private int current = 0;
    [SerializeField] private TextMeshProUGUI textMesh = null;

    public void AddScientist()
    {
        total++;
        current++;
        UpdateText();
    }

    public void KillScientist()
    {
        current--;
        UpdateText();
    }

    private void UpdateText()
    {
        textMesh.SetText(current + "/" + total);
    }
}
