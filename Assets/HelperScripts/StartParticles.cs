using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleFactory : MonoBehaviour
{
    [SerializeField] private GameObject particleSystemToSpawn = null;

    public void InstantiateAt(Transform trans){
        Instantiate<GameObject>(particleSystemToSpawn, trans.position, Quaternion.identity);
    }
}
