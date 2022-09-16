using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

namespace Core.GlobalEvents
{
    public class BoolGlobalEventListener : MonoBehaviour
    {
        [Tooltip("Event to register with.")]
        public BoolGlobalEvent Event;

        [Tooltip("Response to invoke when Event is raised.")]
        public UnityEvent<bool> Response;

        private void OnEnable()
        {
            Event.RegisterListener(this);
        }

        private void OnDisable()
        {
            Event.UnregisterListener(this);
        }

        public void OnEventRaised(bool value)
        {
            Response.Invoke(value);
        }

    }
}