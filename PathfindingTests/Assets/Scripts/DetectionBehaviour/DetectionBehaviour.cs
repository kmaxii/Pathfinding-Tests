using System.Collections.Generic;
using UnityEngine;

namespace DetectionBehaviour
{
    public abstract class DetectionBehaviour : ScriptableObject
    {
        
        private readonly float _detectionDistance = 2f;
        private const int MaximumNeighbours = 600;
        private readonly Collider2D[] _neighborNonAlloc = new Collider2D[MaximumNeighbours];

        public abstract LinkedList<Transform> GetShortestPath(Transform start, Transform end, SphereTransforms sphereTransforms);
        
        protected IEnumerable<Transform> GetNeighbours(Transform transform)
        {
            var size = Physics2D.OverlapCircleNonAlloc(transform.position, _detectionDistance, _neighborNonAlloc);
            
            for (int i = 0; i < size; i++)
            {
                if (_neighborNonAlloc[i].transform.CompareTag("Circle"))
                    yield return _neighborNonAlloc[i].transform;
            }
        }
        
        protected float GetDistance(Vector3 first, Vector3 second)
        {
            return Vector3.Distance(first, second);
            
            float num1 = first.x - second.x;
            float num2 = first.y - second.y;
            return num1 * num1 + num2 * num2;
        }
    }
}
