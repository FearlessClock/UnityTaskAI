using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class BackButtonController : MonoBehaviour
{
    public UnityEngine.Events.UnityEvent OnBackButtonPressedEvent;

    private void Update()
    {
        if (Input.GetKey(KeyCode.Escape))
        {
            OnBackButtonPressed();
        }
    }
    public void OnBackButtonPressed()
    {
        OnBackButtonPressedEvent?.Invoke();
    }
}
