using System.Collections.Generic;
using MaxisGeneralPurpose.Scriptable_objects;
using UnityEngine;
using UnityEngine.Profiling;

namespace ResolutionBehaviour
{
    [CreateAssetMenu(menuName = "Custom/Pathfinding/ResolutionBehaviour/CallVector2IntResolution")]

    public class CallVector2IntResolution : ResolutionBehaviour
    {
    
        [SerializeField] private GameEventWithVector2Int gameEventWithVector2Int;
    
        public override void Resolve(LinkedList<Vector2Int> path)
        {
            Profiler.BeginSample("Resolve", this);

            foreach (var pos in path)
            {
                gameEventWithVector2Int.Raise(pos);
            }
            
            Profiler.EndSample();
        }
    }
}
