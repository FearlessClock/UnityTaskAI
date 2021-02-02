using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreController : MonoBehaviour
{
    [SerializeField] private float currentGlobalScore = 0;
    public static ScoreController instance = null;
    public Action OnScoreUpdate = null;

    public float GetScore => currentGlobalScore;

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this.gameObject);
            return;
        }
    }

    public void UpdateScore(float val)
    {
        currentGlobalScore += val;
        OnScoreUpdate?.Invoke();
    }
}
