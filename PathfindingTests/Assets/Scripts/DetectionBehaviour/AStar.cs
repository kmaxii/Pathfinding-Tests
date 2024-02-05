using System.Collections.Generic;
using UnityEngine;

namespace DetectionBehaviour
{
    [CreateAssetMenu(menuName = "Custom/Pathfinding/Astar")]
    public class AStar : DetectionBehaviour
    {
        
        public bool useHeuristic = true;

        public override LinkedList<Vector2Int> GetShortestPath(Vector2Int start, Vector2Int end)
        {
            var openSet = new PriorityQueue<Vector2Int, float>();
            var cameFrom = new Dictionary<Vector2Int, Vector2Int>();

            var gScore = new Dictionary<Vector2Int, float> { [start] = 0 };
            var fScore = new Dictionary<Vector2Int, float> { [start] = useHeuristic ? GetHeuristic(start, end) : 0 };

            openSet.Enqueue(start, fScore[start]);

            while (openSet.Count > 0)
            {
                var current = openSet.Dequeue();

                if (current.Equals(end))
                    return ReconstructPath(cameFrom, current);

                foreach (var neighbour in GetNeighbours(current))
                {
                    var tentativeGScore = gScore[current] + GetDistance(current, neighbour);
                    var tentativeFScore = tentativeGScore + (useHeuristic ? GetHeuristic(neighbour, end) : 0);

                    if (gScore.ContainsKey(neighbour) && !(tentativeGScore < gScore[neighbour])) continue;
                    cameFrom[neighbour] = current;
                    gScore[neighbour] = tentativeGScore;
                    fScore[neighbour] = tentativeFScore;

                    if (!openSet.Contains(neighbour))
                        openSet.Enqueue(neighbour, fScore[neighbour]);
                }
            }

            return new LinkedList<Vector2Int>();
        }

        private LinkedList<Vector2Int> ReconstructPath(Dictionary<Vector2Int, Vector2Int> cameFrom, Vector2Int current)
        {
            var totalPath = new LinkedList<Vector2Int>();
            totalPath.AddFirst(current);

            while (cameFrom.ContainsKey(current))
            {
                current = cameFrom[current];
                totalPath.AddFirst(current);
            }

            return totalPath;
        }
        
        private float GetHeuristic(Vector2Int position, Vector2Int target)
        {
            return  GetDistance(position, target);
        }
    }
}