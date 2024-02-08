using System.Collections.Generic;
using UnityEngine;

namespace DetectionBehaviour.ChineseJPS
{
    [CreateAssetMenu(menuName = "Custom/Pathfinding/ChineseJPS")]
    public class ChinesJpsDection: DetectionBehaviour

    {
        ChineseJPS chineseJps = new();

        [SerializeField] ChinesIgrid grid;
        public override (LinkedList<Vector2Int>, int nodesExplored) GetShortestPath(Vector2Int start, Vector2Int end)
        {
            var result = chineseJps.Find(grid, start, end);
            
            if (result == null)
            {
                return (new LinkedList<Vector2Int>(), 0);
            }
            
            //Convert the result to a linked list
            var path = new LinkedList<Vector2Int>();
            foreach (var node in result)
            {
                path.AddLast(node);
            }
            return (path, result.Length);
        }
    }
}