using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorController : MonoBehaviour
{
    [SerializeField] private DoorOpeningController doorway = null;
    private bool isDoorOpen = false;
    [SerializeField] private bool defaultOpenState = true;
    [SerializeField] private ToggleDoorsWithWalls toggleDoorsWithWalls = null;
    [SerializeField] private DoorLightController doorLightController = null;
    private bool isDoorActive = true;
    public bool IsDoorActive { get { return isDoorActive; } set { isDoorActive = value; toggleDoorsWithWalls.ToggleDoors(value); ToggleDoor(defaultOpenState); } }
    public bool IsPassable => isDoorOpen || !isDoorActive;
    private bool setupDone = false;

    private void Start()
    {
        GetComponent<ToggleDoorsWithWalls>();
        if(defaultOpenState != isDoorOpen && isDoorActive)
        {
            ToggleDoor();
        }
    }
    public void ToggleDoor()
    {
        if (isDoorActive)
        {
            isDoorOpen = !isDoorOpen;
            doorway.SwitchDoor(isDoorOpen);
            doorLightController.SwitchLight(isDoorOpen);
        }
    }
    public void ToggleDoor(bool value)
    {
        if (isDoorActive)
        {
            isDoorOpen = value;
            doorway.SwitchDoor(value);
            doorLightController.SwitchLight(value);
        }
    }
}
