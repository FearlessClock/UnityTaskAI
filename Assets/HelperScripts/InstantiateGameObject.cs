using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InstantiateGameObject : MonoBehaviour
{
    [SerializeField] private Transform parentTransform = null;
    public void InstantiateGameObjectCallback(GameObject obj)
    {
        Instantiate<GameObject>(obj, this.transform.position,Quaternion.identity, parentTransform) ;
    }
}
