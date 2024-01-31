using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Serialization;

namespace DetectionBehaviour
{
    [CreateAssetMenu(menuName = "Custom/Pathfinding/BidirectionalDijkstra")]
    public class BidirectionalDijkstra : DetectionBehaviour
    {
        private Queue<PathNode> _fromStart;
        private Queue<PathNode> _fromEnd;

        private Dictionary<Vector2Int, PathNode> _openFromStart;
        private Dictionary<Vector2Int, PathNode> _openFromEnd;

        private Dictionary<Vector2Int, PathNode> _closedFromStart;
        private Dictionary<Vector2Int, PathNode> _closedFromEnd;

        private Vector2Int _start;
        private Vector2Int _goal;

        public override LinkedList<Vector2Int> GetShortestPath(Vector2Int start, Vector2Int end)
        {
            _start = start;
            _goal = end;

            _fromStart = new Queue<PathNode>();
            _fromEnd = new Queue<PathNode>();

            _closedFromStart = new Dictionary<Vector2Int, PathNode>();
            _closedFromEnd = new Dictionary<Vector2Int, PathNode>();

            _openFromStart = new Dictionary<Vector2Int, PathNode>();
            _openFromEnd = new Dictionary<Vector2Int, PathNode>();


            return GetPath();
        }

        private LinkedList<Vector2Int> GetPath()
        {
            LinkedList<Vector2Int> bestPath = new LinkedList<Vector2Int>();

            //Special scenario when goal and start can go directly to each other.
            if (GetNeighbours(_start).Contains(_goal))
            {
                bestPath.AddFirst(_start);
                bestPath.AddLast(_goal);
                return bestPath;
            }


            PathNode sharedNode = RunAlgorithmAndGetSharedNode();

            if (sharedNode == null)
                return bestPath;

            sharedNode = CheckForShorterPath(sharedNode);
            
            AddPath(bestPath, _closedFromStart, sharedNode.TilePosition, false);

            bestPath.AddLast(sharedNode.TilePosition);

            AddPath(bestPath, _closedFromEnd, sharedNode.TilePosition, true);


            return bestPath;
        }

        private PathNode CheckForShorterPath(PathNode foundShared)
        {
            float totalCost = GetCostOfSharedNode(foundShared.TilePosition);
            PathNode bestSharedNode = foundShared;

            var sharedNodes = _openFromStart.Where(pair => _closedFromEnd.ContainsKey(pair.Key)).ToList();
            sharedNodes.AddRange(_openFromEnd.Where(pair => _closedFromStart.ContainsKey(pair.Key)));


            for (int i = sharedNodes.Count - 1; i >= 0; i--)
            {
                var keyValuePair = sharedNodes[i];
                MoveToClosedFromStart(keyValuePair.Value);
                MoveToClosedFromEnd(keyValuePair.Value);

                float cost = GetCostOfSharedNode(keyValuePair.Key);

                if (cost < totalCost && keyValuePair.Key != _start && keyValuePair.Key != _goal)
                {
                    bestSharedNode = keyValuePair.Value;
                    totalCost = cost;
                }
            }

            return bestSharedNode;
        }

        private float GetCostOfSharedNode(Vector2Int node)
        {
            return _closedFromEnd[node].DistanceTraveled + _closedFromStart[node].DistanceTraveled;
        }


        private void AddPath(LinkedList<Vector2Int> bestPath, Dictionary<Vector2Int, PathNode> closed, Vector2Int start,
            bool addLast)
        {
            Assert.IsNotNull(closed, "Input Dictionary was null");
            Assert.IsTrue(closed.ContainsKey(start), "closed did not contain key");
            
            if (closed[start].Previous == null)
                return;
            
            Vector2Int previous = closed[start].Previous.TilePosition;
            
          
            
            while (true)
            {
                PathNode node = closed[previous];
                if (addLast)
                {
                    bestPath.AddLast(node.TilePosition);
                }
                else
                    bestPath.AddFirst(node.TilePosition);

                if (node.Previous == null)
                {
                    break;
                }

                previous = node.Previous.TilePosition;
            }
        }


        private PathNode RunAlgorithmAndGetSharedNode()
        {
            AddStartNode();
            AddStartNodeForBackwards();

            while (_fromStart.TryDequeue(out var fromStart) && _fromEnd.TryDequeue(out var fromEnd))
            {

                MoveToClosedFromStart(fromStart);
                CheckNeighbours(fromStart, _closedFromStart, _openFromStart, _fromStart);
                if (_closedFromEnd.ContainsKey(fromStart.TilePosition))
                    return fromStart;


                MoveToClosedFromEnd(fromEnd);

                CheckNeighbours(fromEnd, _closedFromEnd, _openFromEnd, _fromEnd);

                if (_closedFromStart.ContainsKey(fromEnd.TilePosition))
                    return fromEnd;
            }

            return null;
        }


        private void MoveToClosedFromStart(PathNode node)
        {
            _openFromStart.Remove(node.TilePosition);

            _closedFromStart.TryAdd(node.TilePosition, node);
        }

        private void MoveToClosedFromEnd(PathNode node)
        {
            _openFromEnd.Remove(node.TilePosition);

            _closedFromEnd.TryAdd(node.TilePosition, node);
        }

        private void CheckNeighbours(PathNode node, Dictionary<Vector2Int, PathNode> closed,
            Dictionary<Vector2Int, PathNode> open, Queue<PathNode> queue)
        {
            Vector2Int tilePosition = node.TilePosition;

            foreach (var neighbour in GetNeighbours(node.TilePosition)
                .OrderBy(transform => GetDistance(transform, tilePosition)))
            {
                float distance = GetDistance(neighbour, tilePosition);

                //This node neighbour node has already been checked
                if (closed.ContainsKey(neighbour))
                {
                    continue;
                }

                if (!open.ContainsKey(neighbour))
                {
                    PathNode toAdd = new PathNode(neighbour, node.DistanceTraveled + distance, node);
                    queue.Enqueue(toAdd);
                    open.Add(neighbour, toAdd);

                    continue;
                }

                PathNode neighbourNode = open[neighbour];

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

            _fromStart.Enqueue(startNode);
            _openFromStart.Add(_start, startNode);
        }

        private void AddStartNodeForBackwards()
        {
            PathNode endNode = new PathNode(_goal, 0, null);

            _fromEnd.Enqueue(endNode);
            _openFromEnd.Add(_goal, endNode);
        }
    }
}