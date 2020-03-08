using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(TextMeshProUGUI))]
public class UpdateUIIntPlayerPref : MonoBehaviour
{
    [SerializeField] private string prefix = "";

    [SerializeField] private PlayerPrefIntVariable valueVar = null;
    [SerializeField] private string suffix = "";
    private TextMeshProUGUI textToUpdate = null;

    private void OnEnable()
    {
        textToUpdate = GetComponent<TextMeshProUGUI>();
    }

    public void UpdateText(float value)
    {
        if (textToUpdate)
        {
            textToUpdate.SetText(prefix + value.ToString() + suffix);
        }
    }

    public void Update()
    {
        if (valueVar != null)
        {
            UpdateText(valueVar.LatestValue);
        }
    }
}
