using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorController : MonoBehaviour
{
    [SerializeField] private Animator doorAnimator = null;
    private bool isDoorOpen = false;
    [SerializeField] private bool defaultOpenState = true;
    public bool IsPassable => isDoorOpen;

    private void Awake()
    {
        if(defaultOpenState != isDoorOpen)
        {
            ToggleDoor();
        }
    }
    public void ToggleDoor()
    {
        isDoorOpen = !isDoorOpen;
        doorAnimator.SetBool("Open", isDoorOpen) ;
    }
}
