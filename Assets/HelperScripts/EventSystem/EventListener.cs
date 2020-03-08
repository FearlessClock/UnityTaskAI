using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace HelperScripts.EventSystem
{
    public class EventListener : MonoBehaviour
    {
        [SerializeField] private EventScriptable eventScriptable = null;
        public UnityEngine.Events.UnityEvent action = null;
        public void Raise()
        {
            action?.Invoke();
        }

        private void OnEnable()
        {
            eventScriptable.AddListener(this);
        }

        private void OnDisable()
        {
            eventScriptable.RemoveListener(this);
        }
    }
}
