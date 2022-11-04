///
/// Created by Dennis Chen
/// Reference: Ryan's code talked about in https://www.youtube.com/watch?v=raQ3iHhE_Kk
///

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
        private readonly List<StringGlobalEventListener> _eventListeners =
            new List<StringGlobalEventListener>();

        public delegate void StringEvent(string val);
        public event StringEvent OnStringEventCalled;

        public void Raise(string value)
        {
            if(OnStringEventCalled != null)
            {
                OnStringEventCalled.Invoke(value);
            }
            for (int i = _eventListeners.Count - 1; i >= 0; i--)
                _eventListeners[i].OnEventRaised(value); ;
        }

        public string GetSubscribers()
        {
            string subscribers = "Subscribers: \n";
            if (OnStringEventCalled != null)
            {
                for (int i = 0; i < OnStringEventCalled.GetInvocationList().Length; i++)
                {
                    subscribers += OnStringEventCalled.GetInvocationList()[i].Target.ToString() + ": " +
                                   OnStringEventCalled.GetInvocationList()[i].Method.ToString() + "\n";
                }
            }
            return subscribers;
        }

        public void Subscribe(StringEvent stringEvent)
        {
            OnStringEventCalled += stringEvent;
        }

        public void Unsubscribe(StringEvent stringEvent)
        {
            OnStringEventCalled -= stringEvent;
        }

        public void RegisterListener(StringGlobalEventListener listener)
        {
            if (!_eventListeners.Contains(listener))
                _eventListeners.Add(listener);
        }

        public void UnregisterListener(StringGlobalEventListener listener)
        {
            if (_eventListeners.Contains(listener))
                _eventListeners.Remove(listener);
        }

    }
}