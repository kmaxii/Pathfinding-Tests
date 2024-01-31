using MaxisGeneralPurpose.Event;
using UnityEngine;

namespace Scriptable_objects
{
    [CreateAssetMenu(menuName = "Custom/data/float")]
    public class FloatVariable : DataCarrier
    {
        [SerializeField] private float value;

        public float Value
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