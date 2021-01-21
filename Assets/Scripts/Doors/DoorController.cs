using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorController : MonoBehaviour
{
    [SerializeField] private Animator doorAnimator = null;
    private bool isDoorOpen = false;
    [SerializeField] private bool defaultOpenState = true;
    [SerializeField] private ToggleDoorsWithWalls toggleDoorsWithWalls = null;
    private bool isDoorActive = false;
    public bool IsDoorActive { get { return isDoorActive; } set { isDoorActive = value; toggleDoorsWithWalls.ToggleDoors(value);  ToggleDoor(defaultOpenState); } }
    public bool IsPassable => isDoorOpen || !isDoorActive;

    private void Awake()
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
            doorAnimator.SetBool("Open", isDoorOpen);
        }
    }
    public void ToggleDoor(bool value)
    {
        if (isDoorActive)
        {
            isDoorOpen = value;
            doorAnimator.SetBool("Open", value);
        }
    }
}
