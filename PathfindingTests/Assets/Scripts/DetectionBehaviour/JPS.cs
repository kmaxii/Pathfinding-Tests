using System.Collections.Generic;
using Priority_Queue;
using UnityEngine;

namespace DetectionBehaviour {
    [CreateAssetMenu(menuName = "Custom/Pathfinding/JPS")]
    public class JPS : DetectionBehaviour {
        public override (LinkedList<Vector2Int>, int nodesExplored) GetShortestPath(Vector2Int start, Vector2Int end) {
            var openList = new SimplePriorityQueue<Vector2Int, float>();
            var cameFrom = new Dictionary<Vector2Int, Vector2Int>();
            var costSoFar = new Dictionary<Vector2Int, float>();
            int nodesExplored = 0;

            openList.Enqueue(start, 0);
            cameFrom[start] = start;
            costSoFar[start] = 0;

            while (openList.Count > 0) {
                var current = openList.Dequeue();
                nodesExplored++;
                
                if(visualize)
                    color1.Raise(current);

                if (current.Equals(end)) {
                    break;
                }

                foreach (var jumpPoint in FindJumpPoints(current, end)) {
                    var newCost = costSoFar[current] + GetDistance(current, jumpPoint);
                    if (!costSoFar.ContainsKey(jumpPoint) || newCost < costSoFar[jumpPoint]) {
                        costSoFar[jumpPoint] = newCost;
                        float priority = newCost + GetDistance(jumpPoint, end);
                        openList.EnqueueWithoutDuplicates(jumpPoint, priority);
                        cameFrom[jumpPoint] = current;
                    }
                }
            }

            return (BuildPath(cameFrom, start, end), nodesExplored);
        }

        private IEnumerable<Vector2Int> FindJumpPoints(Vector2Int current, Vector2Int end) {
            List<Vector2Int> directions = GetDirections(current, end);
            foreach (var direction in directions) {
                Vector2Int? jumpPoint = FindJumpPoint(current, direction, end);
                if (jumpPoint.HasValue) {
                    yield return jumpPoint.Value;
                }
            }
        }

        private Vector2Int? FindJumpPoint(Vector2Int current, Vector2Int direction, Vector2Int end) {
            Vector2Int next = current + direction;
            if (!mapData.CheckCoordinate(next.x, next.y))
                return null; // Check if next is within the grid and not an obstacle.

            if (next == end) return next; // If the next node is the goal, return it as a jump point.

            // Check for forced neighbors along the diagonal
            if (direction.x != 0 && direction.y != 0) {
                if ((mapData.CheckCoordinate(current.x + direction.x, current.y) &&
                     !mapData.CheckCoordinate(current.x + direction.x, current.y - direction.y)) ||
                    (mapData.CheckCoordinate(current.x, current.y + direction.y) &&
                     !mapData.CheckCoordinate(current.x - direction.x, current.y + direction.y))) {
                    return next;
                }
            }
            else {
                // Check for forced neighbors along the straight paths (horizontal/vertical)
                if (direction.x != 0) {
                    // Horizontal movement
                    if ((!mapData.CheckCoordinate(current.x + direction.x, current.y + 1) &&
                         mapData.CheckCoordinate(current.x, current.y + 1)) ||
                        (!mapData.CheckCoordinate(current.x + direction.x, current.y - 1) &&
                         mapData.CheckCoordinate(current.x, current.y - 1))) {
                        return next;
                    }
                }
                else if (direction.y != 0) {
                    // Vertical movement
                    if ((!mapData.CheckCoordinate(current.x + 1, current.y + direction.y) &&
                         mapData.CheckCoordinate(current.x + 1, current.y)) ||
                        (!mapData.CheckCoordinate(current.x - 1, current.y + direction.y) &&
                         mapData.CheckCoordinate(current.x - 1, current.y))) {
                        return next;
                    }
                }
            }

            // Recursively search for jump points in the direction of movement.
            return FindJumpPoint(next, direction, end);
        }

// This function returns the primary search directions based on the start and end points.
// It simplifies direction calculation for this example. An optimized implementation would adjust these based on specific use cases.
        private List<Vector2Int> GetDirections(Vector2Int current, Vector2Int end) {
            List<Vector2Int> directions = new List<Vector2Int>();

            int dx = Mathf.Clamp(end.x - current.x, -1, 1);
            int dy = Mathf.Clamp(end.y - current.y, -1, 1);

            if (dx != 0) directions.Add(new Vector2Int(dx, 0));
            if (dy != 0) directions.Add(new Vector2Int(0, dy));
            if (dx != 0 && dy != 0) {
                directions.Add(new Vector2Int(dx, dy));
                // Add forced neighbors' directions for diagonals
                if (!mapData.CheckCoordinate(current.x + dx, current.y)) directions.Add(new Vector2Int(0, dy));
                if (!mapData.CheckCoordinate(current.x, current.y + dy)) directions.Add(new Vector2Int(dx, 0));
            }

            return directions;
        }


        private LinkedList<Vector2Int> BuildPath(Dictionary<Vector2Int, Vector2Int> cameFrom, Vector2Int start,
            Vector2Int end) {
            var path = new LinkedList<Vector2Int>();
            var current = end;

            if (!cameFrom.ContainsKey(current))
                return path; // No path exists

            while (!current.Equals(start)) {
                path.AddFirst(current);
                current = cameFrom[current];
            }

            path.AddFirst(start); // Optionally add the start node

            return path;
        }
    }
}