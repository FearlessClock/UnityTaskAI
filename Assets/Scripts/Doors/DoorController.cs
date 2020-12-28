using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorController : MonoBehaviour
{
    [SerializeField] private Animator doorAnimator = null;
    private bool isDoorOpen = false;
    [SerializeField] private bool defaultOpenState = true;
    public bool isDoorActive = false;
    public bool IsPassable => isDoorOpen;

    private void Awake()
    {
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
