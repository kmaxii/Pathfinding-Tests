using System.Collections.Generic;
using Interfaces;
using UnityEngine;

namespace MaxisGeneralPurpose.Scriptable_objects
{
    
    public delegate void SimpleEventHandler();

    [CreateAssetMenu(menuName = "Custom/Event/GameEvent")]
    public class GameEvent : ScriptableObject
    {
        private readonly List<SimpleEventHandler> _listeners = new();

        public void Raise()
        {
            for (int i = _listeners.Count - 1; i >= 0; i--)
            {
                _listeners[i]();
            }
        }

        public void RegisterListener(SimpleEventHandler listener)
        {
            _listeners.Add(listener);
        }

        public void UnregisterListener(SimpleEventHandler listener)
        {
            if (_listeners.Contains(listener))
                _listeners.Remove(listener);
        }
    }
}
