using System.Collections.Generic;
using EpPathFinding.cs;
using PathFinder;
using PathFinder.Grid;
using UnityEngine;

namespace DetectionBehaviour
{
    [CreateAssetMenu(menuName = "Custom/Pathfinding/EpJpsPathfind")]
    public class EpJpsPathfind : DetectionBehaviour
    {
        public override (LinkedList<Vector2Int>, int nodesExplored) GetShortestPath(Vector2Int start, Vector2Int end)
        {

            bool[,] originalData = mapData.map;
            //turn the bool[,] into a bool [][] for the grid
            
            int width = originalData.GetLength(0);
            int height = originalData.GetLength(1);
            bool[][] grid = new bool[width][];
            
            for (int i = 0; i < width; i++)
            {
                grid[i] = new bool[height];
                for (int j = 0; j < height; j++)
                {
                    grid[i][j] = originalData[i, j];
                }
            }
            
            BaseGrid searchGrid = new StaticGrid(width,height, grid);
            
            GridPos startPos=new GridPos(mapData.startPos.x,mapData.startPos.y); 
            GridPos endPos = new GridPos(mapData.endPos.x,mapData.endPos.y);  
            JumpPointParam jpParam = new JumpPointParam(searchGrid,startPos,endPos,  DiagonalMovement.Always, HeuristicMode.EUCLIDEAN);
            
            List<GridPos> resultPathList = JumpPointFinder.FindPath(jpParam); 
            
            LinkedList<Vector2Int> path = new LinkedList<Vector2Int>();
            foreach (var pos in resultPathList)
            {
                path.AddLast(new Vector2Int(pos.x, pos.y));
            }
            return (path, 0);
            
        }

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