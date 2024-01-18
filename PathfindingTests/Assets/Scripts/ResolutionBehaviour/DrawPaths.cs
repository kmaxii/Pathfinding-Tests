using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Profiling;

namespace ResolutionBehaviour
{
    [CreateAssetMenu(menuName = "Custom/ChangeColorOnColision")]
    public class DrawPaths : ResolutionBehaviour
    {
        
        public override void Resolve(LinkedList<Transform> path)
        {
            Profiler.BeginSample("Resolve", this);

            Transform previous = null;
            foreach (var transform in path)
            {
                if (previous != null)
                {
                    Detection.Lines.Add(new Tuple<Vector3, Vector3>(previous.position, transform.position));
                }

                previous = transform;
            }
            
            Profiler.EndSample();

        }
        
 
      



    }
}