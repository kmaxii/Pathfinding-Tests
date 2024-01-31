using System.Collections.Generic;
using UnityEngine;

namespace DetectionBehaviour
{
    [CreateAssetMenu(menuName = "Custom/Pathfinding/Astar")]
    public class AStar : DetectionBehaviour
    {
        private readonly List<PathNode> _open = new List<PathNode>();
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

            while (_open.Count > 0)
            {
                PathNode bestNode = GetBestNode();
                MoveToClosed(bestNode);

                if (bestNode.TilePosition == _goal)
                    break;

                CheckNeighbours(bestNode);
            }
        }

        private PathNode GetBestNode()
        {
            PathNode bestNode = _open[0];
            for (var i = 1; i < _open.Count; i++)
            {
                if (_open[i].EstimatedCost < bestNode.EstimatedCost)
                {
                    bestNode = _open[i];
                }
            }

            return bestNode;
        }


        private void MoveToClosed(PathNode node)
        {
            _open.Remove(node);
            _openDictionary.Remove(node.TilePosition);
            _closed.Add(node.TilePosition, node);
        }

        private void CheckNeighbours(PathNode node)
        {
            foreach (var neighbour in GetNeighbours(node.TilePosition))
            {
                float distance = GetDistance(neighbour, node.TilePosition);

                //This node neighbour node has already been checked
                if (_closed.TryGetValue(neighbour, out var closedNeighbour))
                {
                    //If this way to the node is faster then the current recorded one
                    if (node.DistanceTraveled + distance < closedNeighbour.DistanceTraveled)
                    {
                        closedNeighbour.DistanceTraveled = node.DistanceTraveled + distance;
                        closedNeighbour.Previous = node;

                        MoveFromClosedToOpen(closedNeighbour);
                    }

                    continue;
                }

                if (!_openDictionary.ContainsKey(neighbour))
                {
                    PathNode toAdd = new PathNode(
                        neighbour,
                        node.DistanceTraveled + distance,
                        GetHeuristic(neighbour), node);
                    _open.Add(toAdd);
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


        private void MoveFromClosedToOpen(PathNode node)
        {
            _closed.Remove(node.TilePosition);
            _open.Add(node);
            _openDictionary.Add(node.TilePosition, node);
        }


        private void AddStartNode()
        {
            PathNode startNode = new PathNode(_start, 0, GetHeuristic(_start), null);

            _open.Add(startNode);
            _openDictionary.Add(_start, startNode);
        }

        private float GetHeuristic(Vector2Int position)
        {
            return GetDistance(_goal, position);
        }
    }
}