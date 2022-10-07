using System.Collections.Generic;
using UnityEngine;

namespace Core.GlobalEvents
{
    [CreateAssetMenu]
    public class TransformGlobalEvent : ScriptableObject
    {
        /// <summary>
        /// The list of listeners that this event will notify if it is raised.
        /// </summary>
        private readonly List<TransformGlobalEventListener> _eventListeners =
            new List<TransformGlobalEventListener>();

        public delegate void TransformEvent(Transform val);
        public event TransformEvent OnTransformEventCalled;

        public void Raise(Transform value)
        {
            if(OnTransformEventCalled != null)
            {
                OnTransformEventCalled.Invoke(value);
            }
            for (int i = _eventListeners.Count - 1; i >= 0; i--)
                _eventListeners[i].OnEventRaised(value); ;
        }

        public void Subscribe(TransformEvent transformEvent)
        {
            OnTransformEventCalled += transformEvent;
        }

        public void Unsubscribe(TransformEvent transformEvent)
        {
            OnTransformEventCalled -= transformEvent;
        }

        public void RegisterListener(TransformGlobalEventListener listener)
        {
            if (!_eventListeners.Contains(listener))
                _eventListeners.Add(listener);
        }

        public void UnregisterListener(TransformGlobalEventListener listener)
        {
            if (_eventListeners.Contains(listener))
                _eventListeners.Remove(listener);
        }

    }
}