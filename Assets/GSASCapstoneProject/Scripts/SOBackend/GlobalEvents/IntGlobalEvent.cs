///
/// Created by Dennis Chen
/// Reference: Ryan's code talked about in https://www.youtube.com/watch?v=raQ3iHhE_Kk
///

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
        private readonly List<IntGlobalEventListener> _eventListeners =
            new List<IntGlobalEventListener>();

        public delegate void IntEvent(int val);
        public event IntEvent OnIntEventCalled;

        public void Raise(int value)
        {
            if(OnIntEventCalled != null)
            {
                OnIntEventCalled.Invoke(value);
            }
            for (int i = _eventListeners.Count - 1; i >= 0; i--)
                _eventListeners[i].OnEventRaised(value); ;
        }

        public string GetSubscribers()
        {
            string subscribers = "Subscribers: \n";
            if (OnIntEventCalled != null)
            {
                for (int i = 0; i < OnIntEventCalled.GetInvocationList().Length; i++)
                {
                    subscribers += OnIntEventCalled.GetInvocationList()[i].Target.ToString() + ": " +
                                   OnIntEventCalled.GetInvocationList()[i].Method.ToString() + "\n";
                }
            }
            return subscribers;
        }

        public void Subscribe(IntEvent intEvent)
        {
            OnIntEventCalled += intEvent;
        }

        public void Unsubscribe(IntEvent intEvent)
        {
            OnIntEventCalled -= intEvent;
        }

        public void RegisterListener(IntGlobalEventListener listener)
        {
            if (!_eventListeners.Contains(listener))
                _eventListeners.Add(listener);
        }

        public void UnregisterListener(IntGlobalEventListener listener)
        {
            if (_eventListeners.Contains(listener))
                _eventListeners.Remove(listener);
        }

    }
}