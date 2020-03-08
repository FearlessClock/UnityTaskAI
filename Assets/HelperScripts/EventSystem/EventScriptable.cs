using System;
using System.Collections.Generic;
using UnityEngine;

namespace HelperScripts.EventSystem
{
    [CreateAssetMenu(fileName = "New Event", menuName = "UnityHelperScripts/Event Variable")]
    public class EventScriptable : ScriptableObject
    {
        [SerializeField] private List<EventListener> callbacks = new List<EventListener>();
        public void Call()
        {
            foreach (EventListener action in callbacks)
            {
                action.Raise();
            }
        }

        public void AddListener(EventListener eventListener)
        {
            callbacks.Add(eventListener);
        }

        public void RemoveListener(EventListener eventListener)
        {
            callbacks.Remove(eventListener);
        }
    }
}
