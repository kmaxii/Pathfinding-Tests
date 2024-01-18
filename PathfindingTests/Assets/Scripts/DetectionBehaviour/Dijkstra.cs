using System.Collections.Generic;
using UnityEngine;

namespace DetectionBehaviour
{
    [CreateAssetMenu(menuName = "Custom/Pathfinding/Dijkstra")]
    public class Dijkstra : DetectionBehaviour
    {
        private readonly Queue<PathNode> _open = new Queue<PathNode>();
        private readonly Dictionary<Transform, PathNode> _openDictionary  = new Dictionary<Transform, PathNode>();
        private readonly Dictionary<Transform, PathNode> _closed  = new Dictionary<Transform, PathNode>();

        private Transform _start;
        private Transform _goal;

        [SerializeField] private bool colorExploredNodes;
        [SerializeField] private Color exploredNodesColor  = new Color(214, 93, 177);



        public override LinkedList<Transform> GetShortestPath(Transform start, Transform end,
            SphereTransforms sphereTransforms)
        {
            _start = start;
            _goal = end;
            _open.Clear();
            _closed.Clear();
            _openDictionary.Clear();

            
            return GetPath();
        }

        private LinkedList<Transform> GetPath()
        {
            RunAlgorithm();

            LinkedList<Transform> bestPath = new LinkedList<Transform>();

            Transform previous = _goal;
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

                if (colorExploredNodes)
                    closestNode.TilePosition.GetComponent<SpriteRenderer>().color = exploredNodesColor;

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
                float distance = GetDistance(neighbour.position, node.TilePosition.position);

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