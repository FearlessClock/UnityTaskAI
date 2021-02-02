using System;
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
        current.value = 0;
        UpdateText();
    }

    [SerializeField] private IntVariable total = null;
    [SerializeField] private IntVariable current = null;
    [SerializeField] private TextMeshProUGUI textMesh = null;

    public void AddScientist()
    {
        current.value = current.value+1;
        UpdateText();
    }

    public void KillScientist()
    {
        current.value = current.value - 1;
        UpdateText();
    }

    private void UpdateText()
    {
        textMesh.SetText(current + "/" + total);
    }

    public void ResetScientists()
    {
        current.value = 0;
    }
}
