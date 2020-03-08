using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Move a slider to the values of a Int variable scriptable
/// </summary>
[RequireComponent(typeof(Slider))]
public class IntVarSliderHandler : MonoBehaviour
{
    [SerializeField] private IntVariable intVar = null;
    private Slider slider = null;

    private void Awake()
    {
        slider = GetComponent<Slider>();
        slider.value = (intVar.value);
        intVar.OnValueChanged.AddListener(() => { slider.value = intVar.value; });
    }


}
