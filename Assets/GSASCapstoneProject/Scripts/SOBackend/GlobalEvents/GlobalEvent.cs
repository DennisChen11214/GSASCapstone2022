using System.Collections.Generic;
using UnityEngine;

namespace Core.GlobalEvents
{
    [CreateAssetMenu]
    public class GlobalEvent : ScriptableObject
    {
        /// <summary>
        /// The list of listeners that this event will notify if it is raised.
        /// </summary>
        private readonly List<GlobalEventListener> eventListeners =
            new List<GlobalEventListener>();

        public delegate void GEvent();
        public event GEvent OnGlobalEventCalled;

        public void Raise()
        {
            OnGlobalEventCalled.Invoke();
            for (int i = eventListeners.Count - 1; i >= 0; i--)
                eventListeners[i].OnEventRaised(); ;
        }

        public void Subscribe(GEvent floatEvent)
        {
            OnGlobalEventCalled += floatEvent;
        }

        public void UnSubscribe(GEvent floatEvent)
        {
            OnGlobalEventCalled -= floatEvent;
        }

        public void RegisterListener(GlobalEventListener listener)
        {
            if (!eventListeners.Contains(listener))
                eventListeners.Add(listener);
        }

        public void UnregisterListener(GlobalEventListener listener)
        {
            if (eventListeners.Contains(listener))
                eventListeners.Remove(listener);
        }

    }
}