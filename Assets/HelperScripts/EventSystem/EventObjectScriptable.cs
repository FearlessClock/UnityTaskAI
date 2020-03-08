using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HelperScripts.EventSystem
{
    [CreateAssetMenu(fileName = "New Event", menuName = "UnityHelperScripts/Event system/Event Object Variable", order = 0)]
    public class EventObjectScriptable : ScriptableObject
    {
        [SerializeField] private List<EventObjectListener> callbacks = new List<EventObjectListener>();
        public void Call(object obj)
        {
            foreach (EventObjectListener action in callbacks)
            {
                if(action != null)
                {
                    action.Raise(obj);
                }
            }
        }

        public void RemoveAllListeners()
        {
            callbacks = new List<EventObjectListener>();
        }

        public void AddListener(EventObjectListener eventListener)
        {
            callbacks.Add(eventListener);
        }

        public void RemoveListener(EventObjectListener eventListener)
        {
            callbacks.Remove(eventListener);
        }
    }
}
