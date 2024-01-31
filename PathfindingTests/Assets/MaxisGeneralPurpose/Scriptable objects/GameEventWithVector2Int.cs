using System.Collections.Generic;
using UnityEngine;

namespace MaxisGeneralPurpose.Scriptable_objects
{
    public delegate void Vector2IntCarrierHandler(Vector2Int carrier);

    [CreateAssetMenu(menuName = "Custom/Event/GameEventWithVector2Int")]
    public class GameEventWithVector2Int : ScriptableObject
    {
        private readonly List<Vector2IntCarrierHandler> _listeners = new();

        public void Raise(Vector2Int data)
        {
            for (int i = _listeners.Count - 1; i >= 0; i--)
            {
                _listeners[i](data);
            }
        }

        public void RegisterListener(Vector2IntCarrierHandler listener)
        {
            _listeners.Add(listener);
        }

        public void UnregisterListener(Vector2IntCarrierHandler listener)
        {
            _listeners.Remove(listener);
        }
    }
}