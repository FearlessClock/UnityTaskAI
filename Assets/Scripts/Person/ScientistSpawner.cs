using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScientistSpawner : MonoBehaviour
{
    private GameObject spawnLocation = null;
    [SerializeField] private Human scientistPrefab = null;
    private Human[] humanInstances = null;
    [SerializeField] private IntVariable totalNumberOfScientists = null;
    [SerializeField] private float timeBetweenSpawns = 2;

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
            yield return new WaitForSeconds(timeBetweenSpawns);
        }
    }

    public void RemoveScientists()
    {
        if (humanInstances != null)
        {
            for (int i = 0; i < humanInstances.Length; i++)
            {
                Destroy(humanInstances[i].gameObject);
            }
        }
    }
}
