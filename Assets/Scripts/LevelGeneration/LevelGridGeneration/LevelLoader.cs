using Pieter.GraphTraversal;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelLoader : MonoBehaviour
{
    [SerializeField] private LevelGridGeneration gridGeneration = null;
    [SerializeField] private FireController fireController = null;
    [SerializeField] private RoomGraphHolder roomGraphHolder = null;
    [SerializeField] private TraversalGraphHolder traversalGraphHolder = null;
    [SerializeField] private ScientistSpawner spawner = null;
    [SerializeField] private GameObject tester = null;
    [SerializeField] private Save.SaveSystem saveSystem = null;

    private void Awake()
    {
        roomGraphHolder.Clear();
        traversalGraphHolder.Clear();
        fireController.gameObject.SetActive(false);
        tester.gameObject.SetActive(false);
    }

    public void GenerateLevel()
    {
        StartCoroutine(DelayDeactivation());
        gridGeneration.GenerateLevel();
        StartCoroutine(DelayReactivation());
    }

    public void LoadLab()
    {
        StartCoroutine(DelayDeactivation());
        Save.Lab lab = saveSystem.Load();
        gridGeneration.GenerateLevel(lab);
        StartCoroutine(DelayReactivation());
    }

    private IEnumerator DelayDeactivation()
    {
        roomGraphHolder.Clear();
        traversalGraphHolder.Clear();
        spawner.RemoveScientists();
        fireController.gameObject.SetActive(false);
        tester.gameObject.SetActive(false);
        yield return new WaitForEndOfFrame();
    }

    private IEnumerator DelayReactivation()
    {
        yield return new WaitForEndOfFrame();
        StartCoroutine(spawner.SpawnScientist());
        fireController.gameObject.SetActive(true);
        tester.gameObject.SetActive(true);
        DayCycleController.instance.StartDay();
    }
}
