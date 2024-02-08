using System.Collections.Generic;
using MaxisGeneralPurpose.Scriptable_objects;
using TMPro.EditorUtilities;
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

        protected IEnumerable<Vector2Int> GetNeighbours(Vector2Int position) {
            var neighbours = new List<Vector2Int>();
            var x = position.x;
            var y = position.y;

            int mapLength = mapData.map.GetLength(0);

            if (x > 0 && mapData.CheckCoordinate(x - 1, y))
                neighbours.Add(new Vector2Int(x - 1, y));
            if (x < mapLength - 1 && mapData.CheckCoordinate(x + 1, y))
                neighbours.Add(new Vector2Int(x + 1, y));
            if (y > 0 && mapData.CheckCoordinate(x, y - 1))
                neighbours.Add(new Vector2Int(x, y - 1));
            if (y < mapLength - 1 && mapData.CheckCoordinate(x, y + 1))
                neighbours.Add(new Vector2Int(x, y + 1));

            //Add diagonal neighbours
            if (x > 0 && y > 0 && mapData.CheckCoordinate(x - 1, y - 1))
                neighbours.Add(new Vector2Int(x - 1, y - 1));
            if (x < mapLength - 1 && y < mapLength - 1 &&
                mapData.CheckCoordinate(x + 1, y + 1))
                neighbours.Add(new Vector2Int(x + 1, y + 1));
            if (x > 0 && y < mapLength - 1 && mapData.CheckCoordinate(x - 1, y + 1))
                neighbours.Add(new Vector2Int(x - 1, y + 1));
            if (x < mapLength - 1 && y > 0 && mapData.CheckCoordinate(x + 1, y - 1))
                neighbours.Add(new Vector2Int(x + 1, y - 1));


            return neighbours;
        }

        protected float GetDistance(Vector2Int first, Vector2Int second) {
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

        private float GetDistanceSquared(Vector2Int first, Vector2Int second) {
            float num1 = first.x - second.x;
            float num2 = first.y - second.y;
            return Mathf.Abs(num1) + Mathf.Abs(num2);
        }
    }
}