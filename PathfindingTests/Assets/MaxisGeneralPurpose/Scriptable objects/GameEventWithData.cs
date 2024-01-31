using System.Collections.Generic;
using MaxisGeneralPurpose.Event;
using UnityEngine;

namespace MaxisGeneralPurpose.Scriptable_objects
{
    public delegate void DataCarrierHandler(DataCarrier carrier);

    [CreateAssetMenu(menuName = "Custom/Event/GameEventWithData")]
    public class GameEventWithData : ScriptableObject
    {
        private readonly List<DataCarrierHandler> _listeners = new();

        public void Raise(DataCarrier data)
        {
            for (int i = _listeners.Count - 1; i >= 0; i--)
            {
                _listeners[i](data);
            }
        }

        public void RegisterListener(DataCarrierHandler listener)
        {
            _listeners.Add(listener);
        }

        public void UnregisterListener(DataCarrierHandler listener)
        {
            _listeners.Remove(listener);
        }
    }
}