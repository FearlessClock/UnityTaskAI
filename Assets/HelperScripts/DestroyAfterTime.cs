using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyAfterTime : MonoBehaviour
{
    [SerializeField] private float destroyTime = 0;
    void Start()
    {
        Destroy(this.gameObject, destroyTime);
    }
}
