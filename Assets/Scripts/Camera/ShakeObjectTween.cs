using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class ShakeObjectTween : MonoBehaviour
{
    [SerializeField] private float duration = 1;
    [SerializeField] private float strength = 1;

    public void ShakeObject()
    {
        this.transform.DOShakePosition(duration, strength);
    }

    private void OnDestroy()
    {
        this.transform.DOKill();
    }
}
