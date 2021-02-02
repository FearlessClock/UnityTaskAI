using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScientistSpawner : MonoBehaviour
{
    private GameObject spawnLocation = null;
    [SerializeField] private Human scientistPrefab = null;
    private Human[] humanInstances = null;
    private int activeHumans = 0;
    [SerializeField] private IntVariable totalNumberOfScientists = null;
    [SerializeField] private float timeBetweenSpawns = 2;
    public Action OnNoActiveScientists = null;

    public IEnumerator SpawnScientist()
    {
        if(spawnLocation == null)
        {
            spawnLocation = GameObject.FindGameObjectWithTag("ScientistSpawn");
        }
        humanInstances = new Human[totalNumberOfScientists.value];
        for (int i = 0; i < totalNumberOfScientists.value; i++)
        {
            humanInstances[i] = Instantiate<Human>(scientistPrefab, spawnLocation.transform.position, Quaternion.identity, this.transform);
            humanInstances[i].SetStartPoint(spawnLocation.transform.position);
            activeHumans++;
            humanInstances[i].OnScientistInactive += OnScientistLeaveBuilding;
            yield return new WaitForSeconds(timeBetweenSpawns);
        }
    }

    private void OnScientistLeaveBuilding(Human human)
    {
        activeHumans--;
        Destroy(human.gameObject);
        if (activeHumans <= 0)
        {
            OnNoActiveScientists?.Invoke();
        }
    }

    public void RemoveScientists()
    {
        if (humanInstances != null)
        {
            for (int i = 0; i < humanInstances.Length; i++)
            {
                if(humanInstances[i] != null)
                {
                    Destroy(humanInstances[i].gameObject);
                }
            }
        }
        humanInstances = null;
        activeHumans = 0;
    }
}
