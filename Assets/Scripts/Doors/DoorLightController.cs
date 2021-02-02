using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorLightController : MonoBehaviour
{
    [SerializeField] private Material lightOff = null;
    [SerializeField] private Material lightOn = null;
    [SerializeField] private new Renderer renderer = null;

    public void SwitchLight(bool isOn)
    {
        if (isOn)
        {
            renderer.material = lightOn;
        }
        else
        {
            renderer.material = lightOff;
        }
    }
}
