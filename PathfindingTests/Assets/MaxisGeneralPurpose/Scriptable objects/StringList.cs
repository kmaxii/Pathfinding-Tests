using System.Collections.Generic;
using MaxisGeneralPurpose.Event;
using UnityEngine;

namespace Scriptable_objects
{
    [CreateAssetMenu(menuName = "Custom/data/stringList")]
    public class StringList : DataCarrier
    {
        [SerializeField] public string[] list;
    }
}