using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Profiling;

namespace ResolutionBehaviour
{
    [CreateAssetMenu(menuName = "Custom/Pathfinding/ResolutionBehaviour/DrawPaths")]
    public class DrawPaths : ResolutionBehaviour
    {
        
        public override void Resolve(LinkedList<Vector2Int> path)
        {
            Profiler.BeginSample("Resolve", this);

            Vector2Int previous = Vector2Int.zero;
            foreach (var transform in path)
            {
//                Detection.Lines.Add(new Tuple<Vector2Int, Vector2Int>(previous, transform));

                previous = transform;
            }
            
            Profiler.EndSample();

        }
        
 
      



    }
}