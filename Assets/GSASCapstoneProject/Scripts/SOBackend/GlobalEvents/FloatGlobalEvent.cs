using System.Collections.Generic;
using UnityEngine;

namespace Core.GlobalEvents
{
    [CreateAssetMenu]
    public class FloatGlobalEvent : ScriptableObject
    {
        /// <summary>
        /// The list of listeners that this event will notify if it is raised.
        /// </summary>
        private readonly List<FloatGlobalEventListener> eventListeners =
            new List<FloatGlobalEventListener>();

        public delegate void FloatEvent(float val);
        public event FloatEvent OnFloatEventCalled;

        public void Raise(float value)
        {
            OnFloatEventCalled.Invoke(value);
            for (int i = eventListeners.Count - 1; i >= 0; i--)
                eventListeners[i].OnEventRaised(value); ;
        }

        public void Subscribe(FloatEvent floatEvent)
        {
            OnFloatEventCalled += floatEvent;
        }

        public void UnSubscribe(FloatEvent floatEvent)
        {
            OnFloatEventCalled -= floatEvent;
        }

        public void RegisterListener(FloatGlobalEventListener listener)
        {
            if (!eventListeners.Contains(listener))
                eventListeners.Add(listener);
        }

        public void UnregisterListener(FloatGlobalEventListener listener)
        {
            if (eventListeners.Contains(listener))
                eventListeners.Remove(listener);
        }

    }
}