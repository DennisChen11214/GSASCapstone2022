///
/// Created by Dennis Chen
/// Reference: Ryan's code talked about in https://www.youtube.com/watch?v=raQ3iHhE_Kk
///

using UnityEngine;
using UnityEngine.Events;

namespace Core.GlobalEvents
{
    public class FloatGlobalEventListener : MonoBehaviour
    {
        [Tooltip("Event to register with.")]
        public FloatGlobalEvent Event;

        [Tooltip("Response to invoke when Event is raised.")]
        public UnityEvent<float> Response;

        private void OnEnable()
        {
            Event.RegisterListener(this);
        }

        private void OnDisable()
        {
            Event.UnregisterListener(this);
        }

        public void OnEventRaised(float value)
        {
            Response.Invoke(value);
        }

    }
}