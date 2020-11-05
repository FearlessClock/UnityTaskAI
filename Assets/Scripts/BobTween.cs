using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class BobTween : MonoBehaviour
{
    [SerializeField] private float bobTime = 1;
    [SerializeField] private float bobHeight = 1;
    private float startingHeight = 0;
    Sequence seq = null; 

    private void Awake()
    {
        startingHeight = this.transform.position.y;
        seq = DOTween.Sequence();

        seq.Append(this.transform.DOMoveY(this.transform.position.y + bobHeight, bobTime / 2).SetEase(Ease.InOutSine)).
            Append(this.transform.DOMoveY(startingHeight, bobTime/2).SetEase(Ease.InOutSine)).
            SetLoops(-1);
    }

    private void OnDestroy()
    {
        DOTween.Kill(seq);
    }
}
