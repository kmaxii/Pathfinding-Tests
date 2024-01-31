using System.Collections.Generic;
using UnityEngine;

namespace DetectionBehaviour
{
    [CreateAssetMenu(menuName = "Custom/Pathfinding/Dijkstra")]
    public class Dijkstra : DetectionBehaviour
    {
        private readonly Queue<PathNode> _open = new();
        private readonly Dictionary<Vector2Int, PathNode> _openDictionary = new();
        private readonly Dictionary<Vector2Int, PathNode> _closed = new();

        private Vector2Int _start;
        private Vector2Int _goal;

        public override LinkedList<Vector2Int> GetShortestPath(Vector2Int start, Vector2Int end)
        {
            _start = start;
            _goal = end;
            _open.Clear();
            _closed.Clear();
            _openDictionary.Clear();


            return GetPath();
        }

        private LinkedList<Vector2Int> GetPath()
        {
            RunAlgorithm();

            LinkedList<Vector2Int> bestPath = new LinkedList<Vector2Int>();

            Vector2Int previous = _goal;
            while (true)
            {
                PathNode node = _closed[previous];
                bestPath.AddLast(node.TilePosition);

                if (node.Previous == null)
                {
                    break;
                }

                previous = node.Previous.TilePosition;
            }

            return bestPath;
        }


        private void RunAlgorithm()
        {
            AddStartNode();

            while (_open.TryDequeue(out var closestNode))
            {
                MoveToClosed(closestNode);

                if (closestNode.TilePosition == _goal)
                    break;

                CheckNeighbours(closestNode);
            }
        }


        private void MoveToClosed(PathNode node)
        {
            _openDictionary.Remove(node.TilePosition);


            _closed.Add(node.TilePosition, node);
        }

        private void CheckNeighbours(PathNode node)
        {
            foreach (var neighbour in GetNeighbours(node.TilePosition))
            {
                float distance = GetDistance(neighbour, node.TilePosition);

                //This neighbour node has already been checked
                if (_closed.ContainsKey(neighbour))
                {
                    continue;
                }

                if (!_openDictionary.ContainsKey(neighbour))
                {
                    PathNode toAdd = new PathNode(neighbour, (node.DistanceTraveled + distance), node);
                    _open.Enqueue(toAdd);
                    _openDictionary.Add(neighbour, toAdd);

                    continue;
                }

                PathNode neighbourNode = _openDictionary[neighbour];

                //If this way to the node is faster then the current recorded one
                if (node.DistanceTraveled + distance < neighbourNode.DistanceTraveled)
                {
                    neighbourNode.DistanceTraveled = node.DistanceTraveled + distance;
                    neighbourNode.Previous = node;
                }
            }
        }


        private void AddStartNode()
        {
            PathNode startNode = new PathNode(_start, 0, null);

            _open.Enqueue(startNode);
            _openDictionary.Add(_start, startNode);
        }
    }
}