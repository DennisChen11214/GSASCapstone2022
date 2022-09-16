using System.Collections.Generic;
using UnityEngine;

namespace Core.GlobalEvents
{
    [CreateAssetMenu]
    public class IntGlobalEvent : ScriptableObject
    {
        /// <summary>
        /// The list of listeners that this event will notify if it is raised.
        /// </summary>
        private readonly List<IntGlobalEventListener> eventListeners =
            new List<IntGlobalEventListener>();

        public delegate void IntEvent(int val);
        public event IntEvent OnIntEventCalled;

        public void Raise(int value)
        {
            OnIntEventCalled.Invoke(value);
            for (int i = eventListeners.Count - 1; i >= 0; i--)
                eventListeners[i].OnEventRaised(value); ;
        }

        public void Subscribe(IntEvent intEvent)
        {
            OnIntEventCalled += intEvent;
        }

        public void UnSubscribe(IntEvent intEvent)
        {
            OnIntEventCalled -= intEvent;
        }

        public void RegisterListener(IntGlobalEventListener listener)
        {
            if (!eventListeners.Contains(listener))
                eventListeners.Add(listener);
        }

        public void UnregisterListener(IntGlobalEventListener listener)
        {
            if (eventListeners.Contains(listener))
                eventListeners.Remove(listener);
        }

    }
}