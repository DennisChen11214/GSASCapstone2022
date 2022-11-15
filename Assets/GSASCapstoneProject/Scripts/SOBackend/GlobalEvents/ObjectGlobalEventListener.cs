///
/// Created by Dennis Chen
/// Reference: Ryan's code talked about in https://www.youtube.com/watch?v=raQ3iHhE_Kk
///

using UnityEngine;
using UnityEngine.Events;

namespace Core.GlobalEvents
{
    public class ObjectGlobalEventListener : MonoBehaviour
    {
        [Tooltip("Event to register with.")]
        public ObjectGlobalEvent Event;

        [Tooltip("Response to invoke when Event is raised.")]
        public UnityEvent<Object> Response;

        private void OnEnable()
        {
            Event.RegisterListener(this);
        }

        private void OnDisable()
        {
            Event.UnregisterListener(this);
        }

        public void OnEventRaised(Object value)
        {
            Response.Invoke(value);
        }

    }
}