using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class AfterTimeEvent : MonoBehaviour
{
    [SerializeField] private float time = 0;
    private float timer = 0;
    private bool hasTimedOut = false;
    public UnityEvent OnTimerDone;

    private void OnEnable()
    {
        timer = time;
        hasTimedOut = false;
    }

    private void Update()
    {
        timer -= Time.deltaTime;
        if(timer < 0 && !hasTimedOut)
        {
            hasTimedOut = true;
            LauchEvent();
        }
    }

    private void LauchEvent()
    {
        OnTimerDone?.Invoke();
    }
}
