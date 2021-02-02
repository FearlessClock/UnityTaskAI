using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DayCycleController : MonoBehaviour
{
    public static DayCycleController instance = null;

    private float currentTime = 0; 
    private bool isDayActive = false;
    [Tooltip("Starting time of the work day (hh, mm)")]
    [SerializeField] private Vector2 startingTime = new Vector2(7, 15);
    [SerializeField] private Vector2 endTime = new Vector2(7, 15);
    [SerializeField] private int timeSpeed = 3;
    private float endTimeMinutes = 0;

    public static Action OnEndOfDay = null;

    public bool IsEndOfDay => isDayActive;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this);
            return;
        }
        currentTime = startingTime.y + startingTime.x * 60;
        endTimeMinutes = endTime.y + endTime.x * 60;
    }

    public void StartDay()
    {
        isDayActive = true;
        currentTime = startingTime.y + startingTime.x * 60;
        endTimeMinutes = endTime.y + endTime.x * 60;
    }

    public void EndOfDay()
    {
        isDayActive = false;
        OnEndOfDay?.Invoke();
    }

    private void Update()
    {
        if (!isDayActive)
        {
            return;
        }

        currentTime += Time.deltaTime* timeSpeed;
        if(currentTime > endTimeMinutes)
        {
            EndOfDay();
        }
    }

    public string GetCurrentTimePretty()
    {
        int hours = Mathf.FloorToInt( currentTime / 60);
        int minutes = Mathf.FloorToInt((currentTime - hours * 60));
        return hours.ToString("D2") + " : " + minutes.ToString("D2");
    }
}
