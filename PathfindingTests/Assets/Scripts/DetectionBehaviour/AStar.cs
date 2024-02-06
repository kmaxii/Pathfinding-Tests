using System.Collections.Generic;
using System.Linq;
using Priority_Queue;
using UnityEngine;

namespace DetectionBehaviour
{
    [CreateAssetMenu(menuName = "Custom/Pathfinding/Astar")]
    public class AStar : DetectionBehaviour
    {
        public override (LinkedList<Vector2Int>, int nodesExplored) GetShortestPath(Vector2Int start, Vector2Int end)
        {
            var cameFrom = new Dictionary<Vector2Int, Vector2Int>();
            var costSoFar = new Dictionary<Vector2Int, float>();
            var frontier  = new SimplePriorityQueue <Vector2Int>();
            frontier.Enqueue(start, 0);
            int explored = 0;

            cameFrom[start] = start;
            costSoFar[start] = 0;

            while (frontier.Count > 0)
            {
                explored++;
                var current = frontier.Dequeue();
                if (visualize)
                {
                    color1.Raise(current);
                }

                if (current.Equals(end))
                    break;

                foreach (var next in GetNeighbours(current))
                {
                    float newCost = costSoFar[current] + GetDistance(current, next);
                    if (!costSoFar.ContainsKey(next) || newCost < costSoFar[next])
                    {
                        costSoFar[next] = newCost;
                        float priority = newCost + GetDistance(end, next);
                        frontier.Enqueue(next, priority);
                        cameFrom[next] = current;
                    }
                }
            }

            return (BuildPath(cameFrom, start, end), explored);
        }
        
        private LinkedList<Vector2Int> BuildPath(Dictionary<Vector2Int, Vector2Int> cameFrom, Vector2Int start,
            Vector2Int end)
        {
            var path = new LinkedList<Vector2Int>();
            var current = end;

            if (!cameFrom.ContainsKey(current))
                return path; // No path exists

            while (!current.Equals(start))
            {
                path.AddFirst(current);
                current = cameFrom[current];
            }

            path.AddFirst(start); // Optionally add the start node

            return path;
        }
    }
}