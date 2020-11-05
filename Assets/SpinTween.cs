using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class SpinTween : MonoBehaviour
{
    [SerializeField] private float rotationSpeed = 0;

    private void Awake()
    {
        this.transform.DORotate(new Vector3(0, 360, 0), rotationSpeed, RotateMode.LocalAxisAdd).SetEase(Ease.Linear).SetLoops(-1);
    }
}
