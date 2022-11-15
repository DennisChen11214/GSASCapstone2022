///
/// Created by Dennis Chen
/// Reference: Ryan's code talked about in https://www.youtube.com/watch?v=raQ3iHhE_Kk
///

using System.Collections.Generic;
using UnityEngine;

namespace Core.GlobalEvents
{
    [CreateAssetMenu]
    public class ObjectGlobalEvent : ScriptableObject
    {
        /// <summary>
        /// The list of listeners that this event will notify if it is raised.
        /// </summary>
        private readonly List<ObjectGlobalEventListener> _eventListeners =
            new List<ObjectGlobalEventListener>();

        public delegate void ObjectEvent(Object val);
        public event ObjectEvent OnObjectEventCalled;

        public void Raise(Object value)
        {
            if(OnObjectEventCalled != null)
            {
                OnObjectEventCalled.Invoke(value);
            }
            for (int i = _eventListeners.Count - 1; i >= 0; i--)
                _eventListeners[i].OnEventRaised(value); ;
        }

        public string GetSubscribers()
        {
            string subscribers = "Subscribers: \n";
            if (OnObjectEventCalled != null)
            {
                for (int i = 0; i < OnObjectEventCalled.GetInvocationList().Length; i++)
                {
                    subscribers += OnObjectEventCalled.GetInvocationList()[i].Target.ToString() + ": " +
                                   OnObjectEventCalled.GetInvocationList()[i].Method.ToString() + "\n";
                }
            }
            return subscribers;
        }

        public void Subscribe(ObjectEvent boolEvent)
        {
            OnObjectEventCalled += boolEvent;
        }

        public void Unsubscribe(ObjectEvent boolEvent)
        {
            OnObjectEventCalled -= boolEvent;
        }

        public void RegisterListener(ObjectGlobalEventListener listener)
        {
            if (!_eventListeners.Contains(listener))
                _eventListeners.Add(listener);
        }

        public void UnregisterListener(ObjectGlobalEventListener listener)
        {
            if (_eventListeners.Contains(listener))
                _eventListeners.Remove(listener);
        }

    }
}