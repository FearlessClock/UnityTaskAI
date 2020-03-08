using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MoveSlider : MonoBehaviour
{
    [SerializeField] private Slider slider = null;

    [SerializeField] private FloatVariable amount = null;
    private void Update() {
        slider.value = amount;
    }
}
