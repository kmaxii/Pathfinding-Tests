using System.Collections.Generic;
using UnityEngine;

namespace DetectionBehaviour
{
    public abstract class DetectionBehaviour : ScriptableObject
    {

        [SerializeField] private MapData mapData;

        public abstract LinkedList<Vector2Int> GetShortestPath(Vector2Int start, Vector2Int end);
        
        protected IEnumerable<Vector2Int> GetNeighbours(Vector2Int position)
        {
            var neighbours = new List<Vector2Int>();
            var x = position.x;
            var y = position.y;
            
            if (x > 0 && mapData.CheckCoordinate(x - 1, y))
                neighbours.Add(new Vector2Int(x - 1, y));
            if (x < mapData.map.GetLength(0) - 1 && mapData.CheckCoordinate(x + 1, y))
                neighbours.Add(new Vector2Int(x + 1, y));
            if (y > 0 && mapData.CheckCoordinate(x, y - 1))
                neighbours.Add(new Vector2Int(x, y - 1));
            if (y < mapData.map.GetLength(1) - 1 && mapData.CheckCoordinate(x, y + 1))
                neighbours.Add(new Vector2Int(x, y + 1));
            
            return neighbours;
        }

        protected float GetDistance(Vector2Int first, Vector2Int second)
        {
            return Mathf.Abs(first.x - second.x) + Mathf.Abs(first.y - second.y);
          //  return Mathf.Sqrt(GetDistanceSquared(first, second));
            
            /*float num1 = first.x - second.x;
            float num2 = first.y - second.y;
            return num1 * num1 + num2 * num2;
            */
        }

        private float GetDistanceSquared(Vector2Int first, Vector2Int second)
        {
            float num1 = first.x - second.x;
            float num2 = first.y - second.y;
            return num1 * num1 + num2 * num2;
        }
    }
}
