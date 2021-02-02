using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    private DayCycleController dayCycleController = null;
    private ScientistSpawner scientistSpawner = null;
    public Action OnEndOfDay = null; 
    public Action OnGameOver = null; 

    private void Awake()
    {
        dayCycleController = FindObjectOfType<DayCycleController>();
        scientistSpawner = FindObjectOfType<ScientistSpawner>();

        scientistSpawner.OnNoActiveScientists += CheckIfLossOrEndOfDay;
    }

    private void CheckIfLossOrEndOfDay()
    {
        if (dayCycleController.IsEndOfDay)
        {
            OnEndOfDay?.Invoke();
        }
        else
        {
            OnGameOver?.Invoke();
        }
    }
}
