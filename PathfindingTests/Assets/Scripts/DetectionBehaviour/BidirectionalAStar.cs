using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace DetectionBehaviour
{
    [CreateAssetMenu(menuName = "Custom/Pathfinding/BidirectionalAstar")]
    public class BidirectionalAStar : DetectionBehaviour
    {
        public bool useHeuristic = true;

        public override LinkedList<Vector2Int> GetShortestPath(Vector2Int start, Vector2Int end)
        {
         
            return new LinkedList<Vector2Int>();
        }

   

        private LinkedList<Vector2Int> ConstructCompletePath(
            Dictionary<Vector2Int, Vector2Int> cameFromStart,
            Dictionary<Vector2Int, Vector2Int> cameFromEnd,
            Vector2Int meetingPointStart,
            Vector2Int meetingPointEnd)
        {
            var path = new LinkedList<Vector2Int>();

            // Construct path from start to meeting point
            var temp = meetingPointStart;
            while (cameFromStart.ContainsKey(temp))
            {
                path.AddFirst(temp);
                temp = cameFromStart[temp];
            }
            path.AddFirst(temp); // Add the start

            // If meeting points are different, it means paths crossed without directly connecting.
            if (!meetingPointStart.Equals(meetingPointEnd))
            {
                path.AddLast(meetingPointEnd); // This ensures the connection between two paths
            }

            // Construct path from meeting point to end
            temp = meetingPointEnd;
            while (cameFromEnd.ContainsKey(temp))
            {
                temp = cameFromEnd[temp];
                path.AddLast(temp);
            }

            return path;
        }

        private IEnumerable<Vector2Int> ReconstructPath(Dictionary<Vector2Int, Vector2Int> cameFrom, Vector2Int current, bool reverse = false)
        {
            var path = new List<Vector2Int> { current };

            while (cameFrom.ContainsKey(current))
            {
                current = cameFrom[current];
                path.Add(current);
            }

            if (reverse) path.Reverse();
            return path;
        }
        
        private float GetHeuristic(Vector2Int position, Vector2Int target)
        {
            return  GetDistance(position, target);
        }
    }
}
