﻿using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace DetectionBehaviour
{
    [CreateAssetMenu(menuName = "Custom/Pathfinding/BiDirectionalAstar")]
    public class BiDirectionalAStar : DetectionBehaviour

    {
        public override LinkedList<Vector2Int> GetShortestPath(Vector2Int start, Vector2Int end)
        {
            // Set up for forward search
            var cameFromStart = new Dictionary<Vector2Int, Vector2Int>();
            var costSoFarStart = new Dictionary<Vector2Int, float>();
            var frontierStart = new PriorityQueue<Vector2Int, float>();
            frontierStart.Enqueue(start, 0);
            cameFromStart[start] = start;
            costSoFarStart[start] = 0;

            // Set up for backward search
            var cameFromEnd = new Dictionary<Vector2Int, Vector2Int>();
            var costSoFarEnd = new Dictionary<Vector2Int, float>();
            var frontierEnd = new PriorityQueue<Vector2Int, float>();
            frontierEnd.Enqueue(end, 0);
            cameFromEnd[end] = end;
            costSoFarEnd[end] = 0;

            // Meet node
            Vector2Int? meetNode = null;

            while (frontierStart.Count > 0 && frontierEnd.Count > 0)
            {
                // Forward search step
                if (frontierStart.Count > 0)
                {
                    var currentStart = frontierStart.Dequeue();
                    if (cameFromEnd.ContainsKey(currentStart))
                    {
                        meetNode = currentStart;
                        break;
                    }
                    ExploreNeighbours(currentStart, end, cameFromStart, costSoFarStart, frontierStart);
                }

                // Backward search step
                if (frontierEnd.Count > 0)
                {
                    var currentEnd = frontierEnd.Dequeue();
                    if (cameFromStart.ContainsKey(currentEnd))
                    {
                        meetNode = currentEnd;
                        break;
                    }
                    ExploreNeighbours(currentEnd, start, cameFromEnd, costSoFarEnd, frontierEnd);
                }
            }

            if (meetNode == null) return new LinkedList<Vector2Int>(); // No path found

            return BuildPath(cameFromStart, cameFromEnd, start, end, meetNode.Value);
        }

        private void ExploreNeighbours(Vector2Int current, Vector2Int target,
            Dictionary<Vector2Int, Vector2Int> cameFrom, Dictionary<Vector2Int, float> costSoFar,
            PriorityQueue<Vector2Int, float> frontier)
        {
            foreach (var next in GetNeighbours(current))
            {
                float newCost = costSoFar[current] + GetDistance(current, next);
                if (!costSoFar.ContainsKey(next) || newCost < costSoFar[next])
                {
                    costSoFar[next] = newCost;
                    float priority = newCost + Heuristic(target, next);
                    frontier.Enqueue(next, priority);
                    cameFrom[next] = current;
                }
            }
        }

        private LinkedList<Vector2Int> BuildPath(Dictionary<Vector2Int, Vector2Int> cameFromStart, Dictionary<Vector2Int, Vector2Int> cameFromEnd, Vector2Int start, Vector2Int end, Vector2Int meetNode)
        {
            var pathStart = BuildHalfPath(cameFromStart, start, meetNode);
            var pathEnd = BuildHalfPath(cameFromEnd, end, meetNode);
            pathEnd.RemoveFirst(); // Remove meetNode to avoid duplication
            var path = new LinkedList<Vector2Int>(pathStart.Concat(pathEnd));

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

        private float Heuristic(Vector2Int a, Vector2Int b)
        {
            // Manhattan distance
            return Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y);
        }
    }
}