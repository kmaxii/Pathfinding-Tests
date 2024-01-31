using MaxisGeneralPurpose.Scriptable_objects;
using Scriptable_objects;
using UnityEngine;

namespace MaxisGeneralPurpose.Event
{
    public abstract class DataCarrier : ScriptableObject
    {
        public GameEvent raiseOnValueChanged;

    }
}
