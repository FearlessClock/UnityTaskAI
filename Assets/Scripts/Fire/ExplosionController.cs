using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class ExplosionController : MonoBehaviour
{
    LevelGeneration levelGeneration = null;
    ExplosionPoint[] explosionPoints = null;

    [SerializeField] private float minTimeTillExplosion = 1;
    [SerializeField] private float maxTimeTillExplosion = 10;
    private float timeTillExplosionCounter = 0;

    private NotificationGlobalController notificationGlobalController = null;

    private void Start()
    {
        notificationGlobalController = FindObjectOfType<NotificationGlobalController>();
        if (!notificationGlobalController)
        {
            Debug.LogError("Could not find notification controller");
        }

        levelGeneration = FindObjectOfType<LevelGeneration>();
        levelGeneration.OnDoneLevelGeneration += OnDoneLevel;
        timeTillExplosionCounter = Random.Range(minTimeTillExplosion, maxTimeTillExplosion);
    }

    private void OnDoneLevel()
    {
        explosionPoints = FindObjectsOfType<ExplosionPoint>();
    }

    private void LateUpdate()
    {
        timeTillExplosionCounter -= Time.deltaTime;
        if(timeTillExplosionCounter <= 0)
        {
            explosionPoints[Random.Range(0, explosionPoints.Length)].StartFire();
            notificationGlobalController.SpawnNotification(explosionPoints[Random.Range(0, explosionPoints.Length)].FireStartLocation);
            timeTillExplosionCounter = Random.Range(minTimeTillExplosion, maxTimeTillExplosion); 
        }
    }
}
