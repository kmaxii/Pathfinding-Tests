using MaxisGeneralPurpose.Event;
using Scriptable_objects;
using UnityEngine;

namespace MaxisGeneralPurpose.Scriptable_objects
{
    [CreateAssetMenu(menuName = "Custom/data/int")]
    public class IntVariable : DataCarrier
    {
        [SerializeField] private int value;
        public int Value
        {
            get => value;
            set
            {
                this.value = value;
                if (raiseOnValueChanged)
                    raiseOnValueChanged.Raise();
            }
        }
        
        
        public override string ToString()
        {
            return value.ToString();
        }
    }
}
