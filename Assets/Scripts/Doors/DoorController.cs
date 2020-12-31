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
    public bool IsDoorActive { get { return isDoorActive; } set { isDoorActive = value; toggleDoorsWithWalls.ToggleDoors(value); } }
    public bool IsPassable => isDoorOpen;

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
}
