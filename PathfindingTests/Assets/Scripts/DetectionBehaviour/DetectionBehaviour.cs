using System.Collections.Generic;
using MaxisGeneralPurpose.Scriptable_objects;
using UnityEngine;

namespace DetectionBehaviour
{
    public abstract class DetectionBehaviour : ScriptableObject
    {
        [SerializeField] protected MapData mapData;

        [SerializeField] protected bool visualize;
        [SerializeField] protected GameEventWithVector2Int color1;
        [SerializeField] protected GameEventWithVector2Int color2;

        public abstract (LinkedList<Vector2Int>, int nodesExplored) GetShortestPath(Vector2Int start, Vector2Int end);

        protected IEnumerable<Vector2Int> GetNeighbours(Vector2Int position)
        {
            var neighbours = new List<Vector2Int>();
            var x = position.x;
            var y = position.y;

            int mapWidth = mapData.map.GetLength(0); // Assuming square map for width and height
            int mapHeight = mapData.map.GetLength(1); // Use if map is not square

            // Add cardinal neighbors
            // Up
            if (y < mapHeight - 1 && mapData.CheckCoordinate(x, y + 1))
                neighbours.Add(new Vector2Int(x, y + 1));
            // Right
            if (x < mapWidth - 1 && mapData.CheckCoordinate(x + 1, y))
                neighbours.Add(new Vector2Int(x + 1, y));
            // Down
            if (y > 0 && mapData.CheckCoordinate(x, y - 1))
                neighbours.Add(new Vector2Int(x, y - 1));
            // Left
            if (x > 0 && mapData.CheckCoordinate(x - 1, y))
                neighbours.Add(new Vector2Int(x - 1, y));

            // Add diagonal neighbours
            // Up Right
            if (x < mapWidth - 1 && y < mapHeight - 1 && mapData.CheckCoordinate(x + 1, y + 1) &&
                (mapData.CheckCoordinate(x + 1, y) || mapData.CheckCoordinate(x, y + 1)))
            {
                neighbours.Add(new Vector2Int(x + 1, y + 1));
            }

            // Down Right
            if (x < mapWidth - 1 && y > 0 && mapData.CheckCoordinate(x + 1, y - 1) &&
                (mapData.CheckCoordinate(x + 1, y) || mapData.CheckCoordinate(x, y - 1)))
            {
                neighbours.Add(new Vector2Int(x + 1, y - 1));
            }

            // Down Left
            if (x > 0 && y > 0 && mapData.CheckCoordinate(x - 1, y - 1) &&
                (mapData.CheckCoordinate(x - 1, y) || mapData.CheckCoordinate(x, y - 1)))
            {
                neighbours.Add(new Vector2Int(x - 1, y - 1));
            }

            // Up Left
            if (x > 0 && y < mapHeight - 1 && mapData.CheckCoordinate(x - 1, y + 1) &&
                (mapData.CheckCoordinate(x - 1, y) || mapData.CheckCoordinate(x, y + 1)))
            {
                neighbours.Add(new Vector2Int(x - 1, y + 1));
            }

            return neighbours;


            /*
             *     int tX = iNode.x;
            int tY = iNode.y;
            List<Node> neighbors = new List<Node>();
            bool tS0 = false,
                tD0 = false,
                tS1 = false,
                tD1 = false,
                tS2 = false,
                tD2 = false,
                tS3 = false,
                tD3 = false;

            GridPos pos = new GridPos();
            if (IsWalkableAt(pos.Set(tX, tY - 1)))
            {
                neighbors.Add(GetNodeAt(pos));
                tS0 = true;
            }

            if (IsWalkableAt(pos.Set(tX + 1, tY)))
            {
                neighbors.Add(GetNodeAt(pos));
                tS1 = true;
            }

            if (IsWalkableAt(pos.Set(tX, tY + 1)))
            {
                neighbors.Add(GetNodeAt(pos));
                tS2 = true;
            }

            if (IsWalkableAt(pos.Set(tX - 1, tY)))
            {
                neighbors.Add(GetNodeAt(pos));
                tS3 = true;
            }


            tD0 = tS3 || tS0;
            tD1 = tS0 || tS1;
            tD2 = tS1 || tS2;
            tD3 = tS2 || tS3;


            if (tD0 && IsWalkableAt(pos.Set(tX - 1, tY - 1)))
            {
                neighbors.Add(GetNodeAt(pos));
            }

            if (tD1 && IsWalkableAt(pos.Set(tX + 1, tY - 1)))
            {
                neighbors.Add(GetNodeAt(pos));
            }

            if (tD2 && IsWalkableAt(pos.Set(tX + 1, tY + 1)))
            {
                neighbors.Add(GetNodeAt(pos));
            }

            if (tD3 && IsWalkableAt(pos.Set(tX - 1, tY + 1)))
            {
                neighbors.Add(GetNodeAt(pos));
            }

            return neighbors;
             */
        }


        protected float GetDistance(Vector2Int first, Vector2Int second)
        {
            //Chebyshev_distance
            //return Mathf.Max(Mathf.Abs(first.x - second.x), Mathf.Abs(first.y - second.y));
            //return Mathf.Abs(first.x - second.x) + Mathf.Abs(first.y - second.y);
            //return GetDistanceSquared(first, second);
            //return Mathf.Sqrt(GetDistanceSquared(first, second));
            //return GetDistanceSquared(first, second);
            return Vector2.Distance(first, second);

            /*float num1 = Mathf.Abs(first.x - second.x);
            float num2 = Mathf.Abs(first.y - second.y);
            return num1 + num2;*/
        }

        private float GetDistanceSquared(Vector2Int first, Vector2Int second)
        {
            float num1 = first.x - second.x;
            float num2 = first.y - second.y;
            return Mathf.Abs(num1) + Mathf.Abs(num2);
        }
    }
}