using System;
using System.Collections.Generic;
using UnityEngine;

namespace ResolutionBehaviour
{
    public abstract class ResolutionBehaviour : ScriptableObject
    {
        public abstract void Resolve(LinkedList<Transform> path);
    }
}