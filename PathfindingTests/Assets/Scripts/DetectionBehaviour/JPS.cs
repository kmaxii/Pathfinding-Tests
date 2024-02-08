using System.Collections.Generic;
using Priority_Queue;
using UnityEngine;

namespace DetectionBehaviour {
    [CreateAssetMenu(menuName = "Custom/Pathfinding/JPS")]
    public class JPS : DetectionBehaviour {
        public override (LinkedList<Vector2Int>, int nodesExplored) GetShortestPath(Vector2Int start, Vector2Int end) {
            var cameFrom = new Dictionary<Vector2Int, Vector2Int>();
            var costSoFar = new Dictionary<Vector2Int, float>();
            var frontier = new SimplePriorityQueue<Vector2Int>();
            frontier.Enqueue(start, 0);
            int explored = 0;

            cameFrom[start] = start;
            costSoFar[start] = 0;

            while (frontier.Count > 0) {
                explored++;
                var current = frontier.Dequeue();
                if (visualize) {
                    color1.Raise(current);
                }

                if (current.Equals(end)) {
                    break;
                }

                foreach (var next in FindJumpPoints(current, end)) {
                    if (!cameFrom.ContainsKey(next)) {
                        float newCost = costSoFar[current] + GetDistance(current, next);
                        if (!costSoFar.ContainsKey(next) || newCost < costSoFar[next]) {
                            costSoFar[next] = newCost;
                            float priority = newCost + GetDistance(next, end);
                            frontier.Enqueue(next, priority);
                            cameFrom[next] = current;
                        }
                    }
                }
            }

            return (BuildPath(cameFrom, start, end), explored);
        }

        private IEnumerable<Vector2Int> FindJumpPoints(Vector2Int current, Vector2Int end) {
            var jumpPoints = new List<Vector2Int>();
            var directions = GetDirections(current, end);

            foreach (var dir in directions) {
                var next = Jump(current, dir, end);
                if (next.HasValue) {
                    jumpPoints.Add(next.Value);
                }
            }

            return jumpPoints;
        }

        private Vector2Int? Jump(Vector2Int current, Vector2Int direction, Vector2Int end) {
            Vector2Int next = new Vector2Int(current.x + direction.x, current.y + direction.y);
            if (!mapData.CheckCoordinate(next.x, next.y)) return null; // Outside bounds or not walkable

            if (next.Equals(end) || IsJumpPoint(next, direction)) return next;

            // Check for forced neighbors along the diagonal
            if (direction.x != 0 && direction.y != 0) {
                if ((mapData.CheckCoordinate(current.x, current.y + direction.y) && !mapData.CheckCoordinate(current.x - direction.x, current.y + direction.y)) ||
                    (mapData.CheckCoordinate(current.x + direction.x, current.y) && !mapData.CheckCoordinate(current.x + direction.x, current.y - direction.y))) {
                    return next;
                }
            } else { // Horizontal/vertical movements
                if (direction.x != 0) { // Moving along x
                    if ((mapData.CheckCoordinate(current.x + direction.x, current.y + 1) && !mapData.CheckCoordinate(current.x, current.y + 1)) ||
                        (mapData.CheckCoordinate(current.x + direction.x, current.y - 1) && !mapData.CheckCoordinate(current.x, current.y - 1))) {
                        return next;
                    }
                } else { // Moving along y
                    if ((mapData.CheckCoordinate(current.x + 1, current.y + direction.y) && !mapData.CheckCoordinate(current.x + 1, current.y)) ||
                        (mapData.CheckCoordinate(current.x - 1, current.y + direction.y) && !mapData.CheckCoordinate(current.x - 1, current.y))) {
                        return next;
                    }
                }
            }

            // Continue jumping in the direction if no forced neighbors found
            if (mapData.CheckCoordinate(next.x, next.y)) {
                return Jump(next, direction, end);
            }

            return null;
        }
        
        private bool IsJumpPoint(Vector2Int current, Vector2Int direction) {
            // Check for forced neighbors
            // This will depend on the direction of movement
            if (direction.x != 0 && direction.y == 0) {
                // Moving horizontally
                // Check for forced neighbors above and below
                return (!mapData.CheckCoordinate(current.x, current.y + 1) && mapData.CheckCoordinate(current.x + direction.x, current.y + 1)) ||
                       (!mapData.CheckCoordinate(current.x, current.y - 1) && mapData.CheckCoordinate(current.x + direction.x, current.y - 1));
            } else if (direction.x == 0 && direction.y != 0) {
                // Moving vertically
                // Check for forced neighbors left and right
                return (!mapData.CheckCoordinate(current.x + 1, current.y) && mapData.CheckCoordinate(current.x + 1, current.y + direction.y)) ||
                       (!mapData.CheckCoordinate(current.x - 1, current.y) && mapData.CheckCoordinate(current.x - 1, current.y + direction.y));
            } else if (direction.x != 0 && direction.y != 0) {
                // Moving diagonally
                // Check for forced neighbors in the moving direction and perpendicular to it
                bool forcedHorizontal = (!mapData.CheckCoordinate(current.x - direction.x, current.y) && mapData.CheckCoordinate(current.x - direction.x, current.y + direction.y));
                bool forcedVertical = (!mapData.CheckCoordinate(current.x, current.y - direction.y) && mapData.CheckCoordinate(current.x + direction.x, current.y - direction.y));

                // Also consider the node a jump point if direct horizontal or vertical movement from the current position is blocked,
                // indicating a corner jump point.
                bool isCornerJumpPoint = !mapData.CheckCoordinate(current.x + direction.x, current.y) || !mapData.CheckCoordinate(current.x, current.y + direction.y);

                return forcedHorizontal || forcedVertical || isCornerJumpPoint;
            }

            // Not a jump point by default
            return false;
        }

        

        private List<Vector2Int> GetDirections(Vector2Int current, Vector2Int end) {
            var directions = new List<Vector2Int>();

            int xDirection = System.Math.Sign(end.x - current.x);
            int yDirection = System.Math.Sign(end.y - current.y);

            // Always add horizontal/vertical directions
            if (xDirection != 0) {
                directions.Add(new Vector2Int(xDirection, 0));
            }
            if (yDirection != 0) {
                directions.Add(new Vector2Int(0, yDirection));
            }
            // Add diagonal direction if moving in both axes
            if (xDirection != 0 && yDirection != 0) {
                directions.Add(new Vector2Int(xDirection, yDirection));
            }

            return directions;
        }

        private LinkedList<Vector2Int> BuildPath(Dictionary<Vector2Int, Vector2Int> cameFrom, Vector2Int start, Vector2Int end) {
            var path = new LinkedList<Vector2Int>();
            var current = end;

            if (!cameFrom.ContainsKey(current)) {
                return path; // No path exists
            }

            while (!current.Equals(start)) {
                path.AddFirst(current);
                current = cameFrom[current];
            }

            path.AddFirst(start); // Optionally add the start node

            return path;
        }
    }
}
