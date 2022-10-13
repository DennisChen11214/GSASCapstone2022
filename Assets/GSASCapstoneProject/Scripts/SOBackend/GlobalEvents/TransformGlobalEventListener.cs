///
/// Created by Dennis Chen
/// Reference: Ryan's code talked about in https://www.youtube.com/watch?v=raQ3iHhE_Kk
///

using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

namespace Core.GlobalEvents
{
    public class TransformGlobalEventListener : MonoBehaviour
    {
        [Tooltip("Event to register with.")]
        public TransformGlobalEvent Event;

        [Tooltip("Response to invoke when Event is raised.")]
        public UnityEvent<Transform> Response;

        private void OnEnable()
        {
            Event.RegisterListener(this);
        }

        private void OnDisable()
        {
            Event.UnregisterListener(this);
        }

        public void OnEventRaised(Transform value)
        {
            Response.Invoke(value);
        }

    }
}