using System.Collections.Generic;
using UnityEngine;

namespace Core.GlobalEvents
{
    [CreateAssetMenu]
    public class StringGlobalEvent : ScriptableObject
    {
        /// <summary>
        /// The list of listeners that this event will notify if it is raised.
        /// </summary>
        private readonly List<StringGlobalEventListener> eventListeners =
            new List<StringGlobalEventListener>();

        public delegate void StringEvent(string val);
        public event StringEvent OnStringEventCalled;

        public void Raise(string value)
        {
            OnStringEventCalled.Invoke(value);
            for (int i = eventListeners.Count - 1; i >= 0; i--)
                eventListeners[i].OnEventRaised(value); ;
        }

        public void Subscribe(StringEvent stringEvent)
        {
            OnStringEventCalled += stringEvent;
        }

        public void UnSubscribe(StringEvent stringEvent)
        {
            OnStringEventCalled -= stringEvent;
        }

        public void RegisterListener(StringGlobalEventListener listener)
        {
            if (!eventListeners.Contains(listener))
                eventListeners.Add(listener);
        }

        public void UnregisterListener(StringGlobalEventListener listener)
        {
            if (eventListeners.Contains(listener))
                eventListeners.Remove(listener);
        }

    }
}