using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace HelperScripts.EventSystem
{
    [System.Serializable]
    public class ObjectEvent : UnityEvent<object> { }
    public class EventObjectListener : MonoBehaviour
    {
        [SerializeField] private EventObjectScriptable eventScriptable = null;
        public ObjectEvent action = new ObjectEvent();
        public void Raise(object obj)
        {
            action?.Invoke(obj);
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
