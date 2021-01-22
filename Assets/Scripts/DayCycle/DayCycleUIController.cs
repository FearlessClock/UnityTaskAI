using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DayCycleUIController : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI textMesh = null;

    private void Update()
    {
        textMesh.SetText(DayCycleController.instance.GetCurrentTimePretty());
    }
}
