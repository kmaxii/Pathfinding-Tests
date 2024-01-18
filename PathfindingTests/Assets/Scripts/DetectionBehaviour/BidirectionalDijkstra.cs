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
        [FormerlySerializedAs("colorSearchedNodes")] [SerializeField]
        private bool colorNodes;

        [SerializeField] private Color colorForSearchedNodesFromStart = new Color(214, 93, 177);
        [SerializeField] private Color colorForSearchedNodesFromEnd = new Color(132, 94, 194);
        [SerializeField] private Color combinedNodeColor = new Color(255, 111, 145);

        [Tooltip("Writes the total distance that it would take on all shared nodes in the end")]
        [SerializeField] private bool writeText;

        
        private Queue<PathNode> _fromStart;
        private Queue<PathNode> _fromEnd;

        private Dictionary<Transform, PathNode> _openFromStart;
        private Dictionary<Transform, PathNode> _openFromEnd;

        private Dictionary<Transform, PathNode> _closedFromStart;
        private Dictionary<Transform, PathNode> _closedFromEnd;

        private Transform _start;
        private Transform _goal;

        private SphereTransforms _sphereTransforms;
        

        public override LinkedList<Transform> GetShortestPath(Transform start, Transform end,
            SphereTransforms sphereTransforms)
        {
            _start = start;
            _goal = end;

            _fromStart = new Queue<PathNode>();
            _fromEnd = new Queue<PathNode>();

            _closedFromStart = new Dictionary<Transform, PathNode>();
            _closedFromEnd = new Dictionary<Transform, PathNode>();

            _openFromStart = new Dictionary<Transform, PathNode>();
            _openFromEnd = new Dictionary<Transform, PathNode>();

            _sphereTransforms = sphereTransforms;

            return GetPath();
        }

        private LinkedList<Transform> GetPath()
        {
            LinkedList<Transform> bestPath = new LinkedList<Transform>();

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
                
                if (colorNodes)
                    keyValuePair.Key.GetComponent<SpriteRenderer>().color = combinedNodeColor;


                if (writeText)
                {
                    foreach (var sphere1 in _sphereTransforms.AllColliders.Where(sphere =>
                        sphere.transform == keyValuePair.Key))
                    {
                        sphere1.textMeshPro.text = $"{cost}";
                    }
                }
                

                if (cost < totalCost && keyValuePair.Key != _start && keyValuePair.Key != _goal)
                {
                    bestSharedNode = keyValuePair.Value;
                    totalCost = cost;
                }
            }

            return bestSharedNode;
        }

        private float GetCostOfSharedNode(Transform node)
        {
            return _closedFromEnd[node].DistanceTraveled + _closedFromStart[node].DistanceTraveled;
        }


        private void AddPath(LinkedList<Transform> bestPath, Dictionary<Transform, PathNode> closed, Transform start,
            bool addLast)
        {
            Assert.IsNotNull(closed, "Input Dictionary was null");
            Assert.IsTrue(closed.ContainsKey(start), "closed did not contain key");
            
            if (closed[start].Previous == null)
                return;
            
            Transform previous = closed[start].Previous.TilePosition;
            
          
            
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
                if (colorNodes)
                {
                    fromStart.TilePosition.GetComponent<SpriteRenderer>().color =
                        _closedFromEnd.ContainsKey(fromStart.TilePosition)
                            ? combinedNodeColor
                            : colorForSearchedNodesFromStart;

                    fromEnd.TilePosition.GetComponent<SpriteRenderer>().color =
                        _closedFromStart.ContainsKey(fromStart.TilePosition)
                            ? combinedNodeColor
                            : colorForSearchedNodesFromEnd;
                }

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

        private void CheckNeighbours(PathNode node, Dictionary<Transform, PathNode> closed,
            Dictionary<Transform, PathNode> open, Queue<PathNode> queue)
        {
            Vector3 tilePosition = node.TilePosition.position;

            foreach (var neighbour in GetNeighbours(node.TilePosition)
                .OrderBy(transform => GetDistance(transform.position, tilePosition)))
            {
                float distance = GetDistance(neighbour.position, tilePosition);

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