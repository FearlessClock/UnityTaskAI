using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(TextMeshProUGUI))]
public class UpdateUIString : MonoBehaviour
{
    [SerializeField] private string prefix = "";

    [SerializeField] private StringVariable valueVar = null;
    [SerializeField] private string suffix = "";
    private TextMeshProUGUI textToUpdate = null;

    private void OnEnable()
    {
        textToUpdate = GetComponent<TextMeshProUGUI>();
    }

    public void UpdateText(string value)
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
            UpdateText(valueVar);
        }
    }
}
