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
        private readonly List<GlobalEventListener> _eventListeners =
            new List<GlobalEventListener>();

        public delegate void GEvent();
        public event GEvent OnGlobalEventCalled;

        public void Raise()
        {
            if(OnGlobalEventCalled != null)
            {
                OnGlobalEventCalled.Invoke();
            }
            for (int i = _eventListeners.Count - 1; i >= 0; i--)
                _eventListeners[i].OnEventRaised();
        }

        public void Subscribe(GEvent floatEvent)
        {
            OnGlobalEventCalled += floatEvent;
        }

        public void Unsubscribe(GEvent floatEvent)
        {
            OnGlobalEventCalled -= floatEvent;
        }

        public void RegisterListener(GlobalEventListener listener)
        {
            if (!_eventListeners.Contains(listener))
                _eventListeners.Add(listener);
        }

        public void UnregisterListener(GlobalEventListener listener)
        {
            if (_eventListeners.Contains(listener))
                _eventListeners.Remove(listener);
        }

    }
}