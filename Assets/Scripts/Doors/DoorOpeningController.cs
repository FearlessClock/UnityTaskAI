using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class DoorOpeningController : MonoBehaviour
{
    [SerializeField] private float maxHeight = 20;
    [SerializeField] private float duration = 1;
    [SerializeField] private Vector3 closedPosition;
    [SerializeField] private Vector3 openPosition;
    private bool hasStarted = false;

    private void Start()
    {
        hasStarted = true;
    }

    public void SwitchDoor(bool open)
    {
        if (open)
        {
            OpenDoor();
        }
        else
        {
            CloseDoor();
        }
    }

    public void OpenDoor()
    {
        this.transform.DOLocalMove(closedPosition, duration);
    }

    public void CloseDoor()
    {
        this.transform.DOLocalMove(openPosition, duration);
    }
}
