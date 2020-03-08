using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class StringEvent : UnityEvent<string> { }
public class AnimatorEventActivator : MonoBehaviour
{
    public UnityEvent OnEventReceived;
    public StringEvent OnStringEventReceived;
    public void EventReceived()
    {
        OnEventReceived?.Invoke();
    }

    public void EventStringReceived(string eventName)
    {
        OnStringEventReceived?.Invoke(eventName);
    }
}
