using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorLightController : MonoBehaviour
{
    [SerializeField] private ParticleSystem lightOn = null;

    public void SwitchLight(bool isOn)
    {
        if (isOn)
        {
            lightOn.Play();
        }
        else
        {
            lightOn.Stop();
        }
    }
}
