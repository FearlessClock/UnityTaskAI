using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DestroyCallback : MonoBehaviour
{
    public UnityEngine.Events.UnityEvent OnDestroyThis;
    public void DestroyThis()
    {
        OnDestroyThis?.Invoke();
        Destroy(this.gameObject);
    }
}
