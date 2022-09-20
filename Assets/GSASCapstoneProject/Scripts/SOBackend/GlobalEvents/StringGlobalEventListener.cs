using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

namespace Core.GlobalEvents
{
    public class StringGlobalEventListener : MonoBehaviour
    {
        [Tooltip("Event to register with.")]
        public StringGlobalEvent Event;

        [Tooltip("Response to invoke when Event is raised.")]
        public UnityEvent<string> Response;

        private void OnEnable()
        {
            Event.RegisterListener(this);
        }

        private void OnDisable()
        {
            Event.UnregisterListener(this);
        }

        public void OnEventRaised(string value)
        {
            Response.Invoke(value);
        }

    }
}