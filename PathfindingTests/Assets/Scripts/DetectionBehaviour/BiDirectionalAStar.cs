using System.Collections.Generic;
using System.Linq;
using Priority_Queue;
using UnityEngine;

namespace DetectionBehaviour
{
    [CreateAssetMenu(menuName = "Custom/Pathfinding/BiDirectionalAstar")]
    public class BiDirectionalAStar : DetectionBehaviour
    {
        public override (LinkedList<Vector2Int>, int nodesExplored) GetShortestPath(Vector2Int start, Vector2Int end)
        {
            var cameFromStart = new Dictionary<Vector2Int, Vector2Int>();
            var costSoFarStart = new Dictionary<Vector2Int, float>();
            var frontierStart = new SimplePriorityQueue<Vector2Int>();
            frontierStart.Enqueue(start, 0);
            cameFromStart[start] = start;
            costSoFarStart[start] = 0;

            var cameFromEnd = new Dictionary<Vector2Int, Vector2Int>();
            var costSoFarEnd = new Dictionary<Vector2Int, float>();
            var frontierEnd = new SimplePriorityQueue<Vector2Int>();
            frontierEnd.Enqueue(end, 0);
            cameFromEnd[end] = end;
            costSoFarEnd[end] = 0;

            var potentialMeetNodes = new Dictionary<Vector2Int, float>();

            int explored = 0;

            while (frontierStart.Count > 0 && frontierEnd.Count > 0)
            {
                if (frontierStart.Count > 0)
                {
                    var currentStart = frontierStart.Dequeue();
                    explored++;

                    if (cameFromEnd.ContainsKey(currentStart))
                    {
                        potentialMeetNodes[currentStart] = costSoFarStart[currentStart] + costSoFarEnd[currentStart];
                    }

                    ExploreNeighbours(currentStart, end, cameFromStart, costSoFarStart, frontierStart, true);
                }

                if (frontierEnd.Count > 0)
                {
                    var currentEnd = frontierEnd.Dequeue();
                    explored++;

                    if (cameFromStart.ContainsKey(currentEnd))
                    {
                        potentialMeetNodes[currentEnd] = costSoFarStart[currentEnd] + costSoFarEnd[currentEnd];
                    }

                    ExploreNeighbours(currentEnd, start, cameFromEnd, costSoFarEnd, frontierEnd, false);
                }
            }

            if (potentialMeetNodes.Count == 0) return (new LinkedList<Vector2Int>(), explored);
            var bestMeetNode = potentialMeetNodes.OrderBy(kvp => kvp.Value).First().Key;

            return (BuildPath(cameFromStart, cameFromEnd, start, end, bestMeetNode), explored);
        }

        private void ExploreNeighbours(Vector2Int current, Vector2Int target,
            Dictionary<Vector2Int, Vector2Int> cameFrom, Dictionary<Vector2Int, float> costSoFar,
            SimplePriorityQueue<Vector2Int> frontier, bool isForward)
        {
            foreach (var next in GetNeighbours(current))
            {
                float newCost = costSoFar[current] + GetDistance(current, next);
                if (!costSoFar.ContainsKey(next) || newCost < costSoFar[next])
                {
                    costSoFar[next] = newCost;
                    float priority = newCost + GetDistance(target, next);
                    if (frontier.Contains(next))
                    {
                        frontier.UpdatePriority(next, priority);
                    }
                    else
                    {
                        frontier.Enqueue(next, priority);
                    }
                    cameFrom[next] = current;
                }
            }
        }

        private LinkedList<Vector2Int> BuildPath(Dictionary<Vector2Int, Vector2Int> cameFromStart, Dictionary<Vector2Int, Vector2Int> cameFromEnd, Vector2Int start, Vector2Int end, Vector2Int meetNode)
        {
            var pathStart = BuildHalfPath(cameFromStart, start, meetNode);
            var pathEnd = BuildHalfPath(cameFromEnd, end, meetNode);
            pathEnd.RemoveFirst(); // Avoid duplication of meetNode
            //var path = new LinkedList<Vector2Int>(pathStart.Concat(pathEnd));
            LinkedList<Vector2Int> path = new();

            for (int i = 0; i < pathStart.Count; i++) {
                path.AddLast(pathStart.ElementAt(i));
            }
            for (int i = pathEnd.Count - 1; i >= 0; i--) {
                path.AddLast(pathEnd.ElementAt(i));
            }
            
            return path;
        }

        private LinkedList<Vector2Int> BuildHalfPath(Dictionary<Vector2Int, Vector2Int> cameFrom, Vector2Int start, Vector2Int meetNode)
        {
            var path = new LinkedList<Vector2Int>();
            var current = meetNode;
            while (!current.Equals(start))
            {
                path.AddFirst(current);
                current = cameFrom[current];
            }
            path.AddFirst(start); // Add the start node for the first half, meetNode for the second half
            return path;
        }
    }
}
