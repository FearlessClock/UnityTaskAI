using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CanvasGroup))]
public class CanvasActivation : MonoBehaviour
{
    private CanvasGroup canvasGroup = null;
    private void OnEnable() {
        canvasGroup = GetComponent<CanvasGroup>();
    }
    public void ActivateCanvas(bool isActive){
        canvasGroup.interactable = isActive;
        canvasGroup.blocksRaycasts = isActive;
        canvasGroup.alpha = isActive? 1 : 0;
    }
}
