using System.Collections.Generic;
using Priority_Queue;
using UnityEngine;

namespace DetectionBehaviour
{
    [CreateAssetMenu(menuName = "Custom/Pathfinding/AstarWithJPS")]
    public class AStarWithJPS : DetectionBehaviour
    {
        public override (LinkedList<Vector2Int>, int nodesExplored) GetShortestPath(Vector2Int start, Vector2Int end)
        {
            var cameFrom = new Dictionary<Vector2Int, Vector2Int>();
            var costSoFar = new Dictionary<Vector2Int, float>();
            var frontier = new SimplePriorityQueue<Vector2Int>();
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

                foreach (var next in GetSuccessors(current, start, end))
                {
                    float newCost = costSoFar[current] + GetDistance(current, next); // Assuming GetDistance calculates the straight-line distance
                    if (costSoFar.ContainsKey(next) && !(newCost < costSoFar[next])) continue;
                    costSoFar[next] = newCost;
                    float priority = newCost + GetDistance(end, next); // Heuristic
                    frontier.EnqueueWithoutDuplicates(next, priority);
                    cameFrom[next] = current;
                }
            }

            return (BuildPath(cameFrom, start, end), explored);
        }

        private IEnumerable<Vector2Int> GetSuccessors(Vector2Int current, Vector2Int start, Vector2Int end)
        {
            var successors = new List<Vector2Int>();
            var neighbours = GetNeighbours(current);

            foreach (var neighbour in neighbours)
            {
                var dx = Mathf.Clamp(neighbour.x - current.x, -1, 1);
                var dy = Mathf.Clamp(neighbour.y - current.y, -1, 1);

                var jumpPoint = Jump(current.x, current.y, dx, dy, start, end);
                if (jumpPoint != null)
                {
                    successors.Add(jumpPoint.Value);
                }
            }

            return successors;
        }

        private Vector2Int? Jump(int cx, int cy, int dx, int dy, Vector2Int start, Vector2Int end)
        {
            var nextX = cx + dx;
            var nextY = cy + dy;
            
            //f it's blocked we can't jump here
            if (!mapData.CheckCoordinate(nextX, nextY)) 
                return null;
            
            // If the node is the goal, return it 
            if (nextX == end.x && nextY == end.y)
                return new Vector2Int(nextX, nextY);
            
            var nextPos = new Vector2Int(nextX, nextY);
            
            //Diagonal Case
            if (dx != 0 && dy != 0)
            {
                // Diagonal movement case.
                if ((!IsValid(cx, cy + dy) && IsValid(cx + dx, cy)) || (!IsValid(cx + dx, cy) && IsValid(cx, cy + dy)))
                    return nextPos;

                // Check in horizontal and vertical directions for forced neighbors
                //This is a special case for diagonal direction
                if (!Jump(nextX, nextY, dx, 0, start, end).HasValue 
                    || !Jump(nextX, nextY, 0, dy, start, end).HasValue)
                    return nextPos;
            }
            else
            {
                if (dx != 0)
                {
                    // Horizontal movement case.
                    if ((!IsValid(cx - dx, cy + 1) && IsValid(cx, cy + 1)) || (!IsValid(cx - dx, cy - 1) && IsValid(cx, cy - 1)))
                        return nextPos;
                }
                else if (dy != 0)
                {
                    // Vertical movement case.
                    if ((!IsValid(cx + 1, cy - dy) && IsValid(cx + 1, cy)) ||
                        (!IsValid(cx - 1, cy - dy) && IsValid(cx - 1, cy)))
                        return nextPos;
                }
            }

            // Continue jumping in the current direction.
            return Jump(nextX, nextY, dx, dy, start, end);
        }

        private bool IsValid(int x, int y)
        {
            return mapData.CheckCoordinate(x, y);
        }
        

        // Keep the existing GetNeighbours, BuildPath methods as they are
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